using Content.Shared.PowerCell;
using Robust.Shared.Timing;

namespace Content.Server._ECHO.Battery;

public sealed partial class PassiveBatteryDrainSystem : EntitySystem
{
    [Dependency] private PowerCellSystem _powerCell = default!;
    [Dependency] private IGameTiming _timing = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<PassiveBatteryDrainComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (_timing.CurTime < comp.NextDrain)
                continue;

            comp.NextDrain = _timing.CurTime + TimeSpan.FromSeconds(1);
            _powerCell.TryUseCharge(uid, comp.DrainAmount);
        }
    }
}
