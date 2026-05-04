using Content.Shared._ECHO.Tools;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Map;

namespace Content.Shared._ECHO.Tools;

public abstract class SharedWeldingSparksSystem : EntitySystem
{
    [Dependency] private readonly SharedToolSystem _toolSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WeldingSparksComponent, UseToolEvent>(OnUseTool);
        SubscribeLocalEvent<WeldingSparksComponent, SharedToolSystem.ToolDoAfterEvent>(OnAfterUseTool);

        SubscribeLocalEvent<WeldingSparksComponent, BeforeRangedInteractEvent>(OnBeforeInteract);
    }

    private void OnUseTool(Entity<WeldingSparksComponent> ent, ref UseToolEvent args)
    {
        if (TryComp<ToolComponent>(ent, out var toolComp))
            _toolSystem.PlayToolSound(ent, toolComp, args.User, AudioParams.Default.AddVolume(-2f));

        var spawnLoc = GetSpawnLoc(ent, args.Target);
        if (spawnLoc is not { } loc)
            return;

        DoEffect(ent, args.User, args.Target, args.DoAfterLength, args.DoAfterId, loc);
    }

    private void OnAfterUseTool(Entity<WeldingSparksComponent> ent, ref SharedToolSystem.ToolDoAfterEvent args)
    {
        if (!TryComp<WeldingSparksComponent>(args.Used, out var sparks))
            return;

        StopEffect((args.Used.Value, sparks), args.User, args.DoAfter.Id.Index);
    }


    protected abstract void DoEffect(Entity<WeldingSparksComponent> ent, EntityUid user, EntityUid? target, TimeSpan duration, DoAfterId id, EntityCoordinates spawnLoc);

    protected abstract void StopEffect(Entity<WeldingSparksComponent> ent, EntityUid user, ushort doAfterIdx);

    private EntityCoordinates? GetSpawnLoc(Entity<WeldingSparksComponent> ent, EntityUid? target)
    {
        // If there's a `target` (other than the parent tool), go with that.
        if (target is not null && target != ent.Owner)
            return Transform(target.Value).Coordinates;

        // Otherwise, try to spawn it on the tile where the player clicked.
        if (ent.Comp.LastClickLocation is { } clickLoc && clickLoc.IsValid(EntityManager))
            return clickLoc.SnapToGrid(EntityManager);

        Log.Error("Attempted to spawn weld sparks without a valid spawn location");
        return null;
    }

    // This is a pretty hacky way of putting the spark effect in the right spot when welding a floor tile, since that doesn't pass a `target` arg.
    private void OnBeforeInteract(Entity<WeldingSparksComponent> ent, ref BeforeRangedInteractEvent args)
    {
        if (args.CanReach) // `clickLoc.IsValid()` is checked later in `GetSpawnLoc()`.
            ent.Comp.LastClickLocation = args.ClickLocation;
    }
}
