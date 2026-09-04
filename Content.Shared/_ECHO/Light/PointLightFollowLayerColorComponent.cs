using Robust.Shared.GameStates;

namespace Content.Shared._ECHO.Light;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PointLightFollowLayerColorComponent : Component
{
    [DataField, AutoNetworkedField]
    public string? StringLayer = null;

    [DataField, AutoNetworkedField]
    public Enum? EnumLayer = null;

    [DataField, AutoNetworkedField]
    public int? IdLayer = null;

    [DataField, AutoNetworkedField]
    public bool Enabled = true;
}
