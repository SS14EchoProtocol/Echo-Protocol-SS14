using Content.Server.Power.Components;
using Content.Shared._ECHO.Battery;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.PowerCell;
using Content.Shared.Verbs;
using Robust.Shared.Timing;

namespace Content.Server._ECHO.Battery;

public sealed partial class APCBatteryDrinkerSystem : EntitySystem
{
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private SharedBatterySystem _battery = default!;
    [Dependency] private PowerCellSystem _powerCell = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ApcComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAltVerbs);
        SubscribeLocalEvent<APCBatteryDrinkerComponent, APCDrinkDoAfterEvent>(OnDoAfter);
    }

    private void OnGetAltVerbs(Entity<ApcComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        var user = args.User;
        if (!TryComp<APCBatteryDrinkerComponent>(user, out var drinker))
            return;

        var verb = new AlternativeVerb()
        {
            Text = Loc.GetString("apc-drinker-verb"),
            DoContactInteraction = true,
            Act = () => StartOrBreakDoAfter((user, drinker), ent)
        };

        args.Verbs.Add(verb);
    }

    private void OnDoAfter(Entity<APCBatteryDrinkerComponent> ent, ref APCDrinkDoAfterEvent args)
    {
        if (args.Cancelled)
        {
            ent.Comp.DoAfter = null;
            return;
        }

        if (!_battery.TryUseCharge(args.Target!.Value, ent.Comp.DrainAmount))
        {
            args.Repeat = false;
            ent.Comp.DoAfter = null;
            return;
        }

        if (!_powerCell.TryGetBatteryFromSlot(ent.Owner, out var battery))
            return;

        _battery.ChangeCharge(battery.Value.Owner, ent.Comp.DrainAmount);

        if (_battery.GetCharge(battery.Value.Owner) < battery.Value.Comp.MaxCharge - ent.Comp.DrainSpeed * 1.5f)
        {
            args.Repeat = true;
        }
        else
        {
            args.Repeat = false;
            ent.Comp.DoAfter = null;
        }
    }

    private void StartOrBreakDoAfter(Entity<APCBatteryDrinkerComponent> ent, Entity<ApcComponent> target)
    {
        if (ent.Comp.DoAfter.HasValue)
        {
            _doAfter.Cancel(ent.Comp.DoAfter);
            ent.Comp.DoAfter = null;
        }
        else
        {
            var args = new DoAfterArgs(EntityManager, ent.Owner, ent.Comp.DrainSpeed, new APCDrinkDoAfterEvent(), ent.Owner, target.Owner)
            {
                CancelDuplicate = true,
                BreakOnMove = true,
                BreakOnHandChange = false,
                BreakOnDropItem = false,
            };

            _doAfter.TryStartDoAfter(args, out ent.Comp.DoAfter);
        }
    }
}
