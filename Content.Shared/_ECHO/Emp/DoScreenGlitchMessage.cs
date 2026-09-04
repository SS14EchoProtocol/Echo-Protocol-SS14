using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Emp;

[Serializable, NetSerializable]
public sealed partial class DoScreenGlitchMessage : EntityEventArgs
{
    public readonly float Offset;

    public readonly float Chroma;

    public readonly float Duration;

    public readonly float EffectSpeed;

    public readonly int Segments;

    public readonly float SeedUpdateInterval;

    public readonly bool UpdateSeed;

    public DoScreenGlitchMessage(float offset, float chroma, float duration, float speed, int segments, float seedInterval, bool updateSeed)
    {
        Offset = offset;
        Chroma = chroma;
        Duration = duration;
        EffectSpeed = speed;
        Segments = segments;
        SeedUpdateInterval = seedInterval;
        UpdateSeed = updateSeed;
    }
}
