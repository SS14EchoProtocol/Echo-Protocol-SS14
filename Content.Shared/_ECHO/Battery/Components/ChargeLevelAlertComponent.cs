using Content.Shared.Alert;
using Robust.Shared.Prototypes;

namespace Content.Shared._ECHO.Battery;

[RegisterComponent]
public sealed partial class ChargeLevelAlertComponent : Component
{
    [DataField]
    public ProtoId<AlertPrototype> BatteryAlert = "BorgBattery";

    /// <summary>
    /// The alert for a missing battery.
    /// </summary>
    [DataField]
    public ProtoId<AlertPrototype> NoBatteryAlert = "BorgBatteryNone";
}
