namespace Content.Server._ECHO.Battery;

/// <summary>
/// Simple battery drain that does not depend on anything.
/// </summary>
[RegisterComponent]
public sealed partial class PassiveBatteryDrainComponent : Component
{
    [DataField]
    public float DrainAmount = 1;

    public TimeSpan NextDrain = TimeSpan.Zero;
}
