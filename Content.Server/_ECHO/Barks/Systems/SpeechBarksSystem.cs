using Robust.Shared.Prototypes;
using Content.Shared.ECHO.SpeechBarks;
using Content.Server.Chat.Systems;
using Content.Shared.Chat;
using Robust.Shared.Configuration;
using Content.Shared.ECHO.CCVar;
using Content.Server.Mind;
using Robust.Shared.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server.ECHO.SpeechBarks;

public sealed partial class SpeechBarksSystem : SharedSpeechBarksSystem
{
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private MindSystem _mind = default!;
    [Dependency] private ISharedPlayerManager _player = default!;
    private bool _isEnabled = false;

    public override void Initialize()
    {
        base.Initialize();

        _cfg.OnValueChanged(UCCVars.BarksEnabled, v => _isEnabled = v, true);

        SubscribeLocalEvent<SpeechBarksComponent, EntitySpokeEvent>(OnEntitySpoke);
    }

    private void OnEntitySpoke(EntityUid uid, SpeechBarksComponent component, EntitySpokeEvent args)
    {
        if (!_isEnabled)
            return;

        var ev = new TransformSpeakerBarkEvent(uid, component.Data.Copy());
        RaiseLocalEvent(uid, ev);

        var message = args.ObfuscatedMessage ?? args.Message;
        var soundSpecifier = ev.Data.Sound ?? _proto.Index(ev.Data.Proto).Sound;

        foreach (var ent in _lookup.GetEntitiesInRange(Transform(uid).Coordinates, 10f))
        {
            if (!_mind.TryGetMind(ent, out _, out var mind) || mind.UserId == null || !_player.TryGetSessionById(mind.UserId, out var session))
                continue;

            RaiseNetworkEvent(new PlaySpeechBarksEvent(
                        GetNetEntity(uid),
                        message,
                        soundSpecifier,
                        ev.Data.Pitch,
                        ev.Data.MinVar,
                        ev.Data.MaxVar,
                        args.ObfuscatedMessage != null), session);
        }
    }
}
