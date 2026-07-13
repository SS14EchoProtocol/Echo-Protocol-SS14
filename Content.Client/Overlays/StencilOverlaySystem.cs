using Content.Client.Parallax;
using Content.Client.Weather;
using Content.Shared.StatusEffectNew;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;

namespace Content.Client.Overlays;

public sealed partial class StencilOverlaySystem : EntitySystem
{
<<<<<<< HEAD
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly ParallaxSystem _parallax = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;
    [Dependency] private readonly WeatherSystem _weather = default!;
    [Dependency] private readonly StatusEffectsSystem _status = default!;
=======
    [Dependency] private IOverlayManager _overlay = default!;
    [Dependency] private ParallaxSystem _parallax = default!;
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private SharedMapSystem _map = default!;
    [Dependency] private SpriteSystem _sprite = default!;
    [Dependency] private WeatherSystem _weather = default!;
    [Dependency] private StatusEffectsSystem _status = default!;
>>>>>>> wizzden/master

    public override void Initialize()
    {
        base.Initialize();
        _overlay.AddOverlay(new StencilOverlay(_parallax, _transform, _map, _sprite, _weather, _status));
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlay.RemoveOverlay<StencilOverlay>();
    }
}
