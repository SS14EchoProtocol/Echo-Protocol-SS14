using Robust.Shared.GameStates;

namespace Content.Shared._ECHO.Emp;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ConstantScreenGlitchComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Offset = 0f;

    [DataField, AutoNetworkedField]
    public float Chroma = 0f;

    [DataField, AutoNetworkedField]
    public float EffectSpeed = 1f;

    [DataField, AutoNetworkedField]
    public int Segments = 10;

    [DataField, AutoNetworkedField]
    public float SeedUpdateInterval = 0.25f;

    [DataField, AutoNetworkedField]
    public bool UpdateSeed = true;
}
