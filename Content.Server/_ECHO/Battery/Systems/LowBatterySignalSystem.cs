using Content.Server.Popups;
using Content.Server.Power.EntitySystems;
using Content.Shared.PowerCell;
using Robust.Server.Audio;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server._ECHO.Battery;

public sealed partial class LowBatterySignalSystem : EntitySystem
{
    [Dependency] private AudioSystem _audio = default!;
    [Dependency] private BatterySystem _battery = default!;
    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private PowerCellSystem _powerCell = default!;
    [Dependency] private IGameTiming _timing = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<LowBatterySignalComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (_timing.CurTime < comp.NextSignal)
                continue;

            comp.NextSignal = _timing.CurTime + TimeSpan.FromSeconds(comp.Interval);

            if (!_powerCell.TryGetBatteryFromSlotOrEntity(uid, out var battery))
                continue;

            if (_battery.GetChargeLevel(battery.Value.Owner) > comp.Threshold)
                continue;

            _audio.PlayPvs(comp.Sound, uid);
            _popup.PopupEntity(comp.SelfPopup, uid, uid, Shared.Popups.PopupType.Medium);
            _popup.PopupEntity(comp.SelfPopup, uid, Filter.PvsExcept(uid), true);
        }
    }
}
