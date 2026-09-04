using Content.Shared.Alert;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.DoAfter;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Server._ECHO.Battery;

/// <summary>
/// Allows entity to recharge its battery from APC
/// </summary>
[RegisterComponent]
public sealed partial class FuelBatteryComponent : Component
{
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<ReagentPrototype>, float> ReagentDrains = new();

    [DataField, AutoNetworkedField]
    public float UpdatePeriod = 10f;

    [DataField, AutoNetworkedField]
    public string FuelSolution = "fuel";

    [DataField, AutoNetworkedField]
    public ProtoId<AlertPrototype> Alert = "EchoFuel";

    [ViewVariables]
    public TimeSpan NextUpdate = TimeSpan.Zero;

    [ViewVariables]
    public bool FuelValid = false;
}
