using Content.Shared.Implants.Components;
<<<<<<< HEAD
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
=======
>>>>>>> wizzden/master
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Content.Shared.UserInterface;
using Robust.Shared.Utility;

namespace Content.Server.Store.Systems;

public sealed partial class StoreSystem : SharedStoreSystem
{
<<<<<<< HEAD
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;

=======
>>>>>>> wizzden/master
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StoreComponent, ActivatableUIOpenAttemptEvent>(OnStoreOpenAttempt);
        SubscribeLocalEvent<StoreComponent, BeforeActivatableUIOpenEvent>(BeforeActivatableUiOpen);

        SubscribeLocalEvent<StoreComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<StoreComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<StoreComponent, ComponentShutdown>(OnShutdown);

        SubscribeLocalEvent<RemoteStoreComponent, OpenUplinkImplantEvent>(OnImplantActivate);

        InitializeUi();
        InitializeCommand();
        InitializeRefund();
    }

    private void OnMapInit(EntityUid uid, StoreComponent component, MapInitEvent args)
    {
        RefreshAllListings(component);
        component.StartingMap = Transform(uid).MapUid;

        // Add the bui key if it does not exist already (the check is needed to make sure that we don't overwrite existing InterfaceData).
<<<<<<< HEAD
        if (!_uiSystem.HasUi(uid, StoreUiKey.Key))
            _uiSystem.SetUi(uid, StoreUiKey.Key, new InterfaceData("StoreBoundUserInterface"));
=======
        if (!UI.HasUi(uid, StoreUiKey.Key))
            UI.SetUi(uid, StoreUiKey.Key, new InterfaceData("StoreBoundUserInterface"));
>>>>>>> wizzden/master
    }

    private void OnStartup(EntityUid uid, StoreComponent component, ComponentStartup args)
    {
        // for traitors, because the StoreComponent for the PDA can be added at any time.
        if (MetaData(uid).EntityLifeStage == EntityLifeStage.MapInitialized)
        {
            RefreshAllListings(component);
        }

        var ev = new StoreAddedEvent();
        RaiseLocalEvent(uid, ref ev, true);
    }

    private void OnShutdown(EntityUid uid, StoreComponent component, ComponentShutdown args)
    {
        var ev = new StoreRemovedEvent();
        RaiseLocalEvent(uid, ref ev, true);
    }

    private void OnStoreOpenAttempt(EntityUid uid, StoreComponent component, ActivatableUIOpenAttemptEvent args)
    {
        if (!component.OwnerOnly)
            return;

        if (!Mind.TryGetMind(args.User, out var mind, out _))
            return;

        component.AccountOwner ??= mind;
        DebugTools.Assert(component.AccountOwner != null);

        if (component.AccountOwner == mind)
            return;

        if (!args.Silent)
            Popup.PopupEntity(Loc.GetString("store-not-account-owner", ("store", uid)), uid, args.User);

        args.Cancel();
    }

    private void OnImplantActivate(Entity<RemoteStoreComponent> entity, ref OpenUplinkImplantEvent args)
    {
        if (GetRemoteStore(entity.AsNullable()) is not { } store)
            return;

        ToggleUi(args.Performer, store, store.Comp, entity, entity.Comp);
    }
}
