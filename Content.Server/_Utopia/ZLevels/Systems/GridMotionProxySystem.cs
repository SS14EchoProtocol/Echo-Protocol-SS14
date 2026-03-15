using Content.Server._Utopia.ZLevels.Components;
using Content.Server._Utopia.ZLevels.Events;

namespace Content.Server._Utopia.ZLevels.Systems;

public sealed class GridMotionProxySystem : EntitySystem
{
    [Dependency] private readonly GridSyncSystem _sync = default!;
    [Dependency] private readonly GridThrustSystem _thrust = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<GridMotionProxyComponent, GridMotionChangedEvent>(OnMotionChanged);
        SubscribeLocalEvent<GridMotionProxyComponent, GridMotionCommandEvent>(OnMotionCommand);
    }

    private void OnMotionChanged(EntityUid uid, GridMotionProxyComponent comp, GridMotionChangedEvent ev)
    {
        if (!comp.IsMaster)
            return;

        _sync.Broadcast(comp.SyncGroup, uid, new GridMotionCommandEvent
        {
            LinearDirection = ev.LinearDirection,
            LinearPower = ev.LinearPower,
            AngularPower = ev.AngularPower
        });
    }

    private void OnMotionCommand(EntityUid uid, GridMotionProxyComponent comp, GridMotionCommandEvent ev)
    {
        if (comp.IsMaster)
            return;

        _thrust.Apply(comp.Grid, ev);
    }
}
