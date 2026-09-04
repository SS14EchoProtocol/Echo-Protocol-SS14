using System.Linq;
using Content.Shared.Body;
using Content.Shared.ECHO.SpeechBarks;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameStates;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._ECHO.Customization;

public abstract partial class SharedMidroundCustomizationSystem : EntitySystem
{
    [Dependency] protected SharedVisualBodySystem VisualBody = default!;
    [Dependency] private SharedUserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MidroundCustomizationComponent, ToggleMidroundCustomizationMenuEvent>(OnToggleMenu);

        SubscribeAllEvent<MidroundCustomizationOptionSelectedEvent>(OnRadialOptionSelected);

        Subs.BuiEvents<MidroundCustomizationComponent>(MidroundCustomizationAppearanceUiKey.Key, subs =>
        {
            subs.Event<BoundUIOpenedEvent>(OnBuiOpened);
            subs.Event<MidroundCustomizationSelectMarkingMessage>(OnSelectMarking);
        });
    }

    private void OnToggleMenu(Entity<MidroundCustomizationComponent> ent, ref ToggleMidroundCustomizationMenuEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        if (ent.Comp.RadialOptions.Count <= 0)
        {
            _ui.OpenUi(ent.Owner, MidroundCustomizationAppearanceUiKey.Key, args.Performer);
        }
        else if (ent.Comp.RadialOptions.Count == 1)
        {
            RaiseLocalEvent(new MidroundCustomizationOptionSelectedEvent(GetNetEntity(ent.Owner), ent.Comp.RadialOptions[0]));
        }
        else
        {
            OpenRadialMenu(ent.Comp.RadialOptions);
        }
    }

    private void OnRadialOptionSelected(MidroundCustomizationOptionSelectedEvent args)
    {
        var ent = GetEntity(args.Sender);

        if (args.Option.UiKey != null)
        {
            _ui.TryOpenUi(ent, args.Option.UiKey, ent);
        }

        if (args.Option.Event != null)
        {
            RaiseLocalEvent(ent, args.Option.Event);
        }
    }

    private void OnSelectMarking(Entity<MidroundCustomizationComponent> ent, ref MidroundCustomizationSelectMarkingMessage args)
    {
        if (ent.Comp.AppearanceChangeDuration <= 0f)
        {
            VisualBody.ApplyMarkings(ent.Owner, args.Markings);
            UpdateUi(ent);
        }
        else
        {
            StartChangeDoAfter(ent, args.Markings);
        }
    }

    private void OnBuiOpened(Entity<MidroundCustomizationComponent> ent, ref BoundUIOpenedEvent args)
        => UpdateUi(ent);

    protected virtual void StartChangeDoAfter(Entity<MidroundCustomizationComponent> ent, Dictionary<ProtoId<OrganCategoryPrototype>, Dictionary<HumanoidVisualLayers, List<Marking>>> markings)
    {
    }

    protected virtual void OpenRadialMenu(List<MidroundCustomizationRadialOption> options)
    {
    }

    protected void UpdateUi(Entity<MidroundCustomizationComponent> ent)
    {
        if (!VisualBody.TryGatherMarkingsData(ent.Owner, ent.Comp.AllowedLayers.ToHashSet(), out var profiles, out var markings, out var applied))
            return;

        foreach (var profile in profiles)
        {
            if (!ent.Comp.Organs.Contains(profile.Key))
                profiles.Remove(profile.Key);
        }

        foreach (var marking in markings)
        {
            if (!ent.Comp.Organs.Contains(marking.Key))
                profiles.Remove(marking.Key);
        }

        foreach (var appliedPair in applied)
        {
            if (!ent.Comp.Organs.Contains(appliedPair.Key))
                applied.Remove(appliedPair.Key);
        }

        BarkData? currentBark = null;

        if (TryComp<SpeechBarksComponent>(ent.Owner, out var barks))
            currentBark = barks.Data;

        var state = new MidroundCustomizationBoundUserInterfaceState(profiles, markings, applied, ent.Comp.AllowedLayers, currentBark);
        _ui.SetUiState(ent.Owner, MidroundCustomizationAppearanceUiKey.Key, state);
        _ui.SetUiState(ent.Owner, MidroundCustomizatioBarksUiKey.Key, state);
    }
}
