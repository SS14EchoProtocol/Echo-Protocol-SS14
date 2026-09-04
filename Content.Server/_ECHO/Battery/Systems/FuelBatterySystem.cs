using Content.Server.Power.EntitySystems;
using Content.Shared.Alert;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.PowerCell;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._ECHO.Battery;

public sealed partial class FuelBatterySystem : EntitySystem
{
    [Dependency] private AlertsSystem _alerts = default!;
    [Dependency] private PowerCellSystem _powerCell = default!;
    [Dependency] private BatterySystem _battery = default!;
    [Dependency] private SharedSolutionContainerSystem _solContainer = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FuelBatteryComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<FuelBatteryComponent, SolutionChangedEvent>(OnSolutionChanged);
    }

    private void OnShutdown(Entity<FuelBatteryComponent> ent, ref ComponentShutdown args)
    {
        _alerts.ClearAlert(ent.Owner, ent.Comp.Alert);
    }

    private void OnSolutionChanged(Entity<FuelBatteryComponent> ent, ref SolutionChangedEvent args)
    {
        if (args.Solution.Comp.Id != ent.Comp.FuelSolution)
            return;

        var solution = args.Solution.Comp.Solution;
        var reagents = solution.GetReagentPrototypes(_proto);
        var quantity = 0f;

        foreach (var item in reagents)
        {
            if (!ent.Comp.ReagentDrains.TryGetValue(item.Key, out var requiredVolume))
                continue;

            quantity += item.Value.Float();
        }

        _alerts.ShowAlert(ent.Owner, ent.Comp.Alert, (short)Math.Round((double)(quantity / solution.MaxVolume * 10)));
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<FuelBatteryComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.FuelValid && _powerCell.TryGetBatteryFromSlotOrEntity(uid, out var battery))
            {
                _battery.SetCharge(battery.Value.Owner, battery.Value.Comp.MaxCharge);
            }

            if (comp.NextUpdate > _timing.CurTime)
                continue;

            comp.NextUpdate = _timing.CurTime + TimeSpan.FromSeconds(comp.UpdatePeriod);
            comp.FuelValid = TryUpdateFuel((uid, comp));
        }
    }

    private bool TryUpdateFuel(Entity<FuelBatteryComponent> ent)
    {
        if (!_solContainer.TryGetSolution(ent.Owner, ent.Comp.FuelSolution, out var solutionEnt))
            return false;

        var solution = solutionEnt.Value.Comp.Solution;

        var reagents = solution.GetReagentPrototypes(_proto);

        foreach (var item in reagents)
        {
            if (!ent.Comp.ReagentDrains.TryGetValue(item.Key, out var requiredVolume))
                continue;

            if (item.Value < requiredVolume)
                continue;

            solution.RemoveReagent(item.Key.ID, FixedPoint2.New(requiredVolume), null, true);
            return true;
        }

        return false;
    }
}
