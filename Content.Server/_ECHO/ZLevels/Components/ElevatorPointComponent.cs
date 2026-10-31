using System.Numerics;

namespace Content.Shared._Echo.ZLevels;

[RegisterComponent]
public sealed partial class ElevatorPointComponent : Component
{
    [DataField]
    public ElevatorFloorData FloorData;

    [DataField]
    public string Group = "";

    [DataField(required: true)]
    public Vector2 Offset = Vector2.Zero;

    [DataField(required: true)]
    public string GridPath = "";
}
