using Robust.Shared.GameStates;

namespace Content.Server._ECHO.Emp;

[RegisterComponent]
public sealed partial class ScreenGlitchOnEmpComponent : Component
{
    [DataField]
    public float Offset = 1f;

    [DataField]
    public float Chroma = 1f;

    [DataField]
    public float Duration = 2f;

    [DataField]
    public float EffectSpeed = 1f;

    [DataField]
    public int Segments = 10;

    [DataField]
    public float SeedUpdateInterval = 0.25f;

    [DataField]
    public bool UpdateSeed = true;
}
