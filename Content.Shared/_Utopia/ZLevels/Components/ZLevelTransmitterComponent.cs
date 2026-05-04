using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._Utopia.ZLevels.Transmission.Components;

/// <summary>
/// Базовый Z-передатчик.
/// Может быть на гриде или просто на карте.
/// </summary>
[RegisterComponent]
public sealed partial class ZLevelTransmitterComponent : Component
{
    [DataField]
    public bool UseGrid = true;

    [DataField]
    public bool AllowUp = true;

    [DataField]
    public bool AllowDown = true;

    [DataField]
    public float Range = 0.01f;
}