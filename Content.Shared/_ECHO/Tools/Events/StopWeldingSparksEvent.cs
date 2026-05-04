using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Tools;

[Serializable, NetSerializable]
public sealed partial class StopWeldingSparksEvent(NetEntity tool, ushort doAfterId) : EntityEventArgs
{
    public readonly NetEntity Tool = tool;
    public readonly ushort DoAfterIdx = doAfterId;
}
