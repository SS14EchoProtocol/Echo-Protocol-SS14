namespace Content.Shared._ECHO.Radio;

[RegisterComponent]
public sealed partial class BatteryIntrinsicRadioReceiverComponent : Component
{
    /// <summary>
    /// Charge at which and lower entity will stop receiving radio messages
    /// </summary>
    [DataField]
    public float ChargeThreshold = 0f;
}
