using Content.Shared.Destructible.Thresholds;
using Robust.Shared.Audio;

namespace Content.Server._ECHO.Battery;

/// <summary>
/// Sending the periodic signal sound and pop-up when entity has low battery charge level.
/// </summary>
[RegisterComponent]
public sealed partial class LowBatterySignalComponent : Component
{
    [DataField]
    public float Threshold = .1f;

    [DataField]
    public string SelfPopup = "";

    [DataField]
    public string OthersPopup = "";

    [DataField]
    public SoundSpecifier? Sound;

    [DataField]
    public float Interval = 8;

    public TimeSpan NextSignal = TimeSpan.Zero;
}
