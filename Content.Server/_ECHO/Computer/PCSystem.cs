using Content.Server.CartridgeLoader;
using Content.Server.GameTicking.Events;
using Content.Shared._ECHO.Computer;
using Content.Shared.CartridgeLoader;
using Content.Shared.GameTicking;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared._ECHO.Extensions;
using System.Text;
using Content.Server.Mind;
using Content.Shared.Roles;
using Robust.Shared.Containers;
using Content.Shared.Players;

namespace Content.Server._ECHO.Computer;

public sealed class PCSystem : SharedPCSystem
{
    [Dependency] private readonly CartridgeLoaderSystem _cartridgeLoader = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public readonly Dictionary<ComputerUserData, ProtoId<ComputerAccessPrototype>> GlobalUsers = new();
    public readonly Dictionary<ComputerUserData, ProtoId<ComputerAccessPrototype>> LocalUsers = new();

    public const string UsernameMindMemoryKey = "computers-username";
    public const string PasswordMindMemoryKey = "computers-password";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundStartingEvent>(OnRoundStart);
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawn);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRestartCleanup);

        SubscribeLocalEvent<PCComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<PCComponent, EntInsertedIntoContainerMessage>(OnContainerInsert);
        SubscribeLocalEvent<PCComponent, EntRemovedFromContainerMessage>(OnContainerRemove);

        SubscribeLocalEvent<PCComponent, BoundUIOpenedEvent>(OnUiOpen);
        SubscribeLocalEvent<PCComponent, PCLoginUiMessage>(OnLogin);
        SubscribeLocalEvent<PCComponent, PCLogOutMessage>(OnLogOut);
        SubscribeLocalEvent<PCComponent, PCTurnOffMessage>(OnTurnOff);
    }

    private void OnRoundStart(RoundStartingEvent args)
    {
        var prototypes = _proto.EnumeratePrototypes<ComputerAccessPrototype>();

        foreach (var item in prototypes)
        {
            if (item.IsGlobal || !item.Names.HasValue)
                continue;

            var name = _random.Pick(_proto.Index(item.Names.Value).Values);
            var dat = new ComputerUserData($"{name}{_random.Next(10, 99)}", GeneratePassword(item));
            LocalUsers.Add(dat, item.ID);
        }
    }
    private void OnPlayerSpawn(PlayerSpawnCompleteEvent args)
    {
        if (args.JobId == null)
            return;

        var job = _proto.Index<JobPrototype>(args.JobId);

        if (job.ComputerAccess == null)
            return;

        var access = _proto.Index<ComputerAccessPrototype>(job.ComputerAccess);

        if (!access.IsGlobal)
            return;

        string name = args.Profile.Name.Replace(" ", "").Replace("'", "").ToEngTranslit();
        var dat = new ComputerUserData($"{name}{_random.Next(10, 99)}", GeneratePassword(access));
        GlobalUsers.Add(dat, access.ID);

        if (_mind.TryGetMind(args.Player, out _, out var mind))
        {
            mind.Memory[UsernameMindMemoryKey] = Loc.GetString("computer-username-memory", ("name", dat.Username));
            mind.Memory[PasswordMindMemoryKey] = Loc.GetString("computer-password-memory", ("password", dat.Password));
        }
    }

    private void OnRestartCleanup(RoundRestartCleanupEvent args)
    {
        GlobalUsers.Clear();
        LocalUsers.Clear();
    }

    private void OnMapInit(Entity<PCComponent> ent, ref MapInitEvent args)
    {
        UpdateUi(ent);
    }

    private void OnContainerInsert(Entity<PCComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        UpdateUi(ent);
    }

    private void OnContainerRemove(Entity<PCComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        UpdateUi(ent);
    }

    private void OnUiOpen(Entity<PCComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateUi(ent);
    }

    private void OnLogin(Entity<PCComponent> ent, ref PCLoginUiMessage args)
    {
        var userData = new ComputerUserData(args.Username, args.Password);

        if (GlobalUsers.TryGetValue(userData, out var globalAccess))
        {
            ent.Comp.CurrentUser = new(args.Username, globalAccess);
        }

        else if (LocalUsers.TryGetValue(userData, out var localAccess) && ent.Comp.AllowedLocalUsers.Contains(localAccess))
        {
            ent.Comp.CurrentUser = new(args.Username, localAccess);
        }

        UpdateUi(ent);
    }

    private void OnLogOut(Entity<PCComponent> ent, ref PCLogOutMessage args)
    {
        ent.Comp.CurrentUser = null;
        UpdateUi(ent);
    }

    private void OnTurnOff(Entity<PCComponent> ent, ref PCTurnOffMessage args)
    {
        ToggleComputer(ent, null);
        UpdateUi(ent);
    }

    private string GeneratePassword(ComputerAccessPrototype localAccess)
    {
        var keyword = Loc.GetString(_random.Pick(_proto.Index(localAccess.PasswordKeywords).Values));
        StringBuilder sb = new();

        var doUnderline = _random.Prob(.5f);

        if (doUnderline && _random.Prob(.2f))
        {
            sb.Append('_');
            doUnderline = false;
        }

        sb.Append(keyword);

        if (doUnderline)
            sb.Append('_');

        sb.Append(_random.Next(100, 999).ToString());
        return sb.ToString();
    }

    public override void UpdateUi(Entity<PCComponent> ent)
    {
        base.UpdateUi(ent);

        if (!TryComp<CartridgeLoaderComponent>(ent.Owner, out var loader))
            return;

        var state = new PCBoundUserInterfaceState(ent.Comp.CurrentUser, GetNetEntity(loader.ActiveProgram), _cartridgeLoader.GetAvailablePrograms(ent.Owner));
        UI.SetUiState(ent.Owner, PCBoundUiKey.Key, state);
    }
}
