using Content.Shared._ECHO.Tools;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Server._ECHO.Tools;

public sealed class WeldingSparksSystem : SharedWeldingSparksSystem
{
    protected override void DoEffect(Entity<WeldingSparksComponent> ent, EntityUid user, EntityUid? target, TimeSpan duration, DoAfterId id, EntityCoordinates spawnLoc)
    {
        if (!target.HasValue)
            return;

        var filter = Filter.PvsExcept(user);
        var ev = new SpawnWeldingSparksEvent(GetNetEntity(ent.Owner), GetNetEntity(target.Value), GetNetCoordinates(spawnLoc), id.Index, duration);
        RaiseNetworkEvent(ev, filter);
    }

    protected override void StopEffect(Entity<WeldingSparksComponent> ent, EntityUid user, ushort doAfterIdx)
    {
        var filter = Filter.PvsExcept(user);
        var ev = new StopWeldingSparksEvent(GetNetEntity(ent.Owner), doAfterIdx);
        RaiseNetworkEvent(ev, filter);
    }
}
