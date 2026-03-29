using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Tools;

/// <summary>
/// Raised by <c>WeldingSparksSystem</c> after it's spawned the sparks effect if there was a target to spawn them on.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class SpawnWeldingSparksEvent(NetEntity tool, NetEntity targetEnt, NetCoordinates targetPos, ushort doAfterId, TimeSpan duration) : EntityEventArgs
{
    public readonly NetEntity Tool = tool;
    public readonly NetEntity TargetEnt = targetEnt;
    public readonly NetCoordinates TargetPos = targetPos;
    public readonly ushort DoAfterIdx = doAfterId;
    public readonly TimeSpan Duration = duration;
}
