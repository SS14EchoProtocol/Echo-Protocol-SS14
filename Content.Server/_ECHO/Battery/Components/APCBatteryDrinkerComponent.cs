using Content.Shared.DoAfter;
using Robust.Shared.GameStates;

namespace Content.Server._ECHO.Battery;

/// <summary>
/// Allows entity to recharge its battery from APC
/// </summary>
[RegisterComponent]
public sealed partial class APCBatteryDrinkerComponent : Component
{
    [DataField]
    public float DrainSpeed = 1f;

    [DataField]
    public float DrainAmount = 15f;

    [ViewVariables]
    public DoAfterId? DoAfter;
}
