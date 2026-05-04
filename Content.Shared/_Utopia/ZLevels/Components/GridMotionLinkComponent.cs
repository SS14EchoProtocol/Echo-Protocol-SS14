using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared._Utopia.ZLevels.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class GridMotionLinkComponent : Component
{
    [DataField, AutoNetworkedField]
    public string GroupId = string.Empty;

    [DataField, AutoNetworkedField]
    public Vector2 Offset = Vector2.Zero;

    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public EntityUid Root;
}
