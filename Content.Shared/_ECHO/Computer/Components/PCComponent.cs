using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._ECHO.Computer;

[RegisterComponent, NetworkedComponent]
public sealed partial class PCComponent : Component
{
    [DataField]
    public bool AllowGlobalUsers = true;

    [DataField]
    public List<ProtoId<ComputerAccessPrototype>> AllowedLocalUsers = new();

    [ViewVariables(VVAccess.ReadWrite)]
    public ComputerLoginData? CurrentUser;
}
