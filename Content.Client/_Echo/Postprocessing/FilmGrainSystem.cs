using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;

namespace Content.Client._Echo.Postprocessing;

public sealed class FilmGrainSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private float _amount = 1f;
    private FilmGrainOverlay _overlay = new();

    public const float DefaultGrainAmount = 0.1f;

    public override void Initialize()
    {
        base.Initialize();

        _cfg.OnValueChanged(EchoCCVars.FilmGrain, SetOverlayEnabled, true);
        _cfg.OnValueChanged(EchoCCVars.FilmGrainAmount, SetGrainAmount, true);
    }

    public override void Shutdown()
    {
        base.Shutdown();
        SetOverlayEnabled(false);

        _cfg.UnsubValueChanged(EchoCCVars.FilmGrain, SetOverlayEnabled);
        _cfg.UnsubValueChanged(EchoCCVars.FilmGrainAmount, SetGrainAmount);
    }

    private void SetOverlayEnabled(bool enabled)
    {
        if (_overlayMan.HasOverlay<FilmGrainOverlay>() == enabled)
            return;

        if (enabled)
        {
            _overlay.GrainAmount = DefaultGrainAmount * _amount;
            _overlayMan.AddOverlay(_overlay);
        }
        else
        {
            _overlayMan.RemoveOverlay(_overlay);
        }
    }

    private void SetGrainAmount(float value)
    {
        _amount = value;
        _overlay.GrainAmount = DefaultGrainAmount * _amount;
    }
}
