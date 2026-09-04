using Content.Server.Actions;
using Content.Server.DoAfter;
using Content.Server.ECHO.SpeechBarks;
using Content.Shared._ECHO.Customization;
using Content.Shared.Body;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.Prototypes;

namespace Content.Server._ECHO.Customization;

public sealed partial class MidroundCustomizationSystem : SharedMidroundCustomizationSystem
{
    [Dependency] private ActionsSystem _actions = default!;
    [Dependency] private DoAfterSystem _doAFter = default!;
    [Dependency] private SpeechBarksSystem _barks = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MidroundCustomizationComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<MidroundCustomizationComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<MidroundCustomizationComponent, ApplyMidroundCustomizationMarkingsDoAfterEvent>(OnApplyDoAfter);

        Subs.BuiEvents<MidroundCustomizationComponent>(MidroundCustomizatioBarksUiKey.Key, subs =>
        {
            subs.Event<MidroundCustomizationSetBarkMessage>(OnSetBark);
        });
    }

    private void OnMapInit(Entity<MidroundCustomizationComponent> ent, ref MapInitEvent args)
    {
        _actions.AddAction(ent.Owner, ref ent.Comp.MenuAction, ent.Comp.ActionId);
        UpdateUi(ent);
    }

    private void OnShutdown(Entity<MidroundCustomizationComponent> ent, ref ComponentShutdown args)
    {
        _actions.RemoveAction(ent.Comp.MenuAction);
    }

    private void OnApplyDoAfter(Entity<MidroundCustomizationComponent> ent, ref ApplyMidroundCustomizationMarkingsDoAfterEvent args)
    {
        ent.Comp.AppearanceChangeDoAfter = null;
        if (args.Cancelled)
            return;

        foreach (var (organ, markings) in args.Markings)
        {
            if (!ent.Comp.Organs.Contains(organ))
            {
                args.Markings.Remove(organ);
                continue;
            }

            foreach (var layer in markings.Keys)
            {
                if (!ent.Comp.AllowedLayers.Contains(layer))
                    markings.Remove(layer);
            }
        }

        VisualBody.ApplyMarkings(ent.Owner, args.Markings);
        UpdateUi(ent);
    }

    private void OnSetBark(Entity<MidroundCustomizationComponent> ent, ref MidroundCustomizationSetBarkMessage args)
    {
        _barks.SetBarkData(ent.Owner, args.NewBark);
    }

    protected override void StartChangeDoAfter(Entity<MidroundCustomizationComponent> ent, Dictionary<ProtoId<OrganCategoryPrototype>, Dictionary<HumanoidVisualLayers, List<Marking>>> markings)
    {
        if (ent.Comp.AppearanceChangeDoAfter != null)
        {
            _doAFter.Cancel(ent.Comp.AppearanceChangeDoAfter.Value);
        }

        var ev = new ApplyMidroundCustomizationMarkingsDoAfterEvent(markings);
        var doAfterArgs = new DoAfterArgs(EntityManager, ent.Owner, ent.Comp.AppearanceChangeDuration, ev, ent.Owner)
        {
            BreakOnHandChange = false,
            BreakOnMove = false
        };

        _doAFter.TryStartDoAfter(doAfterArgs, out ent.Comp.AppearanceChangeDoAfter);
    }
}
