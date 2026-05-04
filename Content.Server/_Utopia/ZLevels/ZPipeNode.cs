using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Nodes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server._Utopia.ZLevels.Nodes;

[DataDefinition]
public sealed partial class ZPipeNode : PipeNode
{
    [DataField(required: true)]
    public ZNodeDirection ZDirection;
}

[DataDefinition]
public sealed partial class ZCableNode : CableNode
{
    [DataField(required: true)]
    public ZNodeDirection ZDirection;
}

public enum ZNodeDirection
{
    Up,
    Down
}
