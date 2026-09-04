using Content.Shared._ECHO.Emp;
using Content.Shared.CCVar;
using Content.Shared.Emp;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._Echo.Postprocessing;

public sealed partial class ScreenGlitchSystem : EntitySystem
{
    [Dependency] private IOverlayManager _overlayMan = default!;
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private IGameTiming _timing = default!;

    private ScreenGlitchOverlay _overlay = new();
    private GlitchFlashData? _lastGlitch;
    private EntityQuery<ConstantScreenGlitchComponent> _constQuery;

    public override void Initialize()
    {
        base.Initialize();

        _constQuery = GetEntityQuery<ConstantScreenGlitchComponent>();

        SubscribeLocalEvent<ScreenGlitchComponent, LocalPlayerAttachedEvent>(OnLocalPlayerAttached);
        SubscribeLocalEvent<ScreenGlitchComponent, LocalPlayerDetachedEvent>(OnLocalPlayerDetached);

        SubscribeNetworkEvent<DoScreenGlitchMessage>(OnEmp);
    }

    private void OnLocalPlayerAttached(Entity<ScreenGlitchComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnLocalPlayerDetached(Entity<ScreenGlitchComponent> ent, ref LocalPlayerDetachedEvent args)
    {
        _overlayMan.RemoveOverlay(_overlay);
    }

    private void OnEmp(DoScreenGlitchMessage args)
    {
        _lastGlitch = new(args.SeedUpdateInterval, args.Offset, args.Chroma, args.EffectSpeed, args.Segments,
                          args.UpdateSeed, _timing.CurTime, _timing.CurTime + TimeSpan.FromSeconds(args.Duration));
    }

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        if (_player.LocalEntity is not { Valid: true } player)
            return;

        if (_constQuery.TryComp(player, out var constant))
        {
            _overlay.MaxOffset = constant.Offset;
            _overlay.Chroma = constant.Chroma;
            _overlay.EffectSpeed = constant.EffectSpeed;

            _overlay.SeedUpdateInterval = constant.SeedUpdateInterval;
            _overlay.UpdateSeed = constant.UpdateSeed;
            _overlay.Segments = constant.Segments;
            return;
        }

        if (_lastGlitch == null)
        {
            _overlay.MaxOffset = 0f;
            _overlay.Chroma = 0f;
            return;
        }

        var duration = _lastGlitch.Value.EndTime - _lastGlitch.Value.StartTime;
        var curTime = _timing.CurTime - _lastGlitch.Value.StartTime;
        var lerpAmount = MathF.Min(1f, (float)(curTime.TotalSeconds / duration.TotalSeconds));

        _overlay.MaxOffset = MathHelper.Lerp(_lastGlitch.Value.Offset, 0f, lerpAmount);
        _overlay.Chroma = MathHelper.Lerp(_lastGlitch.Value.Chroma, 0f, lerpAmount);
        _overlay.EffectSpeed = _lastGlitch.Value.EffectSpeed;

        _overlay.SeedUpdateInterval = _lastGlitch.Value.Interval;
        _overlay.UpdateSeed = _lastGlitch.Value.UpdateSeed;
        _overlay.Segments = _lastGlitch.Value.Segments;

        if (lerpAmount == 1)
            _lastGlitch = null;
    }

    private struct GlitchFlashData
    {
        public float Interval;
        public float Offset;
        public float Chroma;
        public float EffectSpeed;
        public int Segments;
        public bool UpdateSeed;
        public TimeSpan StartTime;
        public TimeSpan EndTime;

        public GlitchFlashData(float interval, float offset, float chroma, float speed, int segments,
                               bool updateSeed, TimeSpan startTime, TimeSpan endTime)
        {
            Interval = interval;
            Offset = offset;
            Chroma = chroma;
            EffectSpeed = speed;
            Segments = segments;
            UpdateSeed = updateSeed;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
