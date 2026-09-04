using Robust.Shared.GameStates;

namespace Content.Shared._ECHO.Battery;

/// <summary>
/// Scales entity move speed dependent on battery charge level
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class ChargeMoveSpeedThresholdsComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public Dictionary<float, float> SpeedModifierThresholds = default!;
}
