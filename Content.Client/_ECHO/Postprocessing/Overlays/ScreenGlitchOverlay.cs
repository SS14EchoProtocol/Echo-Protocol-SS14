using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._Echo.Postprocessing;

public sealed partial class ScreenGlitchOverlay : Overlay
{
    [Dependency] private IPrototypeManager _prototype = default!;
    [Dependency] private IGameTiming _timing = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;

    private readonly ShaderInstance _shader;
    private TimeSpan _lastSeedUpdate = TimeSpan.Zero;
    private float _seed = 1f;

    public float MaxOffset = 0f;
    public float Chroma = 1f;
    public float EffectSpeed = 1f;
    public int Segments = 10;

    public float SeedUpdateInterval = 0.25f;
    public bool UpdateSeed = true;

    public ScreenGlitchOverlay()
    {
        IoCManager.InjectDependencies(this);

        ProtoId<ShaderPrototype> shader = "Glitch";

        _shader = _prototype.Index(shader).InstanceUnique();

        ZIndex = (int) Shared.DrawDepth.DrawDepth.Overlays;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        var handle = args.WorldHandle;

        if (UpdateSeed && _lastSeedUpdate + TimeSpan.FromSeconds(SeedUpdateInterval) <= _timing.CurTime)
        {
            _seed++;
            _lastSeedUpdate = _timing.CurTime;
        }

        _shader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _shader.SetParameter("SEGMENTS_COUNT", Segments);
        _shader.SetParameter("GAME_TIME", (float)_timing.CurTime.TotalSeconds);
        _shader.SetParameter("SPEED", EffectSpeed);
        _shader.SetParameter("OFFSET", MaxOffset * 0.01f);
        _shader.SetParameter("CHROMA", Chroma * 0.01f);
        _shader.SetParameter("SEED", _seed);

        handle.UseShader(_shader);
        handle.DrawRect(args.WorldBounds, Color.White);
        handle.UseShader(null);
    }
}
