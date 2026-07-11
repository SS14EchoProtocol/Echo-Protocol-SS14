using Robust.Shared.GameStates;

namespace Content.Shared._Echo.Dirt;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class DirtVisualsComponent : Component
{
    public const string DirtSolution = "dirt";
    public const string DirtLayer = "dirt";

    [DataField, AutoNetworkedField]
    public Dictionary<DirtAmount, float> DirtThresholds = new()
    {
        { DirtAmount.None, 0f },
        { DirtAmount.Light, 5f },
        { DirtAmount.Medium, 10f },
        { DirtAmount.Heavy, 15f }
    };

    [DataField(required: true), AutoNetworkedField]
    public Dictionary<DirtAmount, PrototypeLayerData> DirtLayers = new();
}

public enum DirtAmount : int
{
    None = 0,
    Light = 1,
    Medium = 2,
    Heavy = 3
}
