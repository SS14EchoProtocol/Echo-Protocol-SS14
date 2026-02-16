using Robust.Shared.Prototypes;

namespace Content.Shared.ECHO.SpeechBarks;

public abstract class SharedSpeechBarksSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public const string DefaultBark = "Human1";

    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// Applies bark data to an entity's SpeechBarksComponent.
    /// Resolves the sound from the bark prototype.
    /// </summary>
    public void SetBarkData(EntityUid uid, BarkData data, SpeechBarksComponent? comp = null)
    {
        if (!Resolve(uid, ref comp, false))
            return;

        comp.Data = data;
        comp.Data.Sound = _proto.Index(comp.Data.Proto).Sound;
    }
}
