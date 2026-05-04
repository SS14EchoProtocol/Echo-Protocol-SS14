using Robust.Shared.GameObjects;

namespace Content.Server._Utopia.ZLevels.Components;

[RegisterComponent]
public sealed partial class GridMotionProxyComponent : Component
{
    public EntityUid Grid;
    public EntityUid SyncGroup;
    public bool IsMaster;
}
