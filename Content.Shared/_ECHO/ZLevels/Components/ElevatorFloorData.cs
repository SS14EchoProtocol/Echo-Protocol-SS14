using Content.Shared.Access;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Echo.ZLevels;

[Serializable, NetSerializable, DataDefinition]
public sealed partial class ElevatorFloorData
{
    [DataField(required: true)]
    public string Name = "";

    [DataField(required: true)]
    public int Priority = 0;

    [DataField]
    public ProtoId<AccessLevelPrototype>? Access;
}
