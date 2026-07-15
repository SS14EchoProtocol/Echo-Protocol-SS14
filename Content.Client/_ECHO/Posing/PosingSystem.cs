using Content.Shared._ECHO.Extensions;
using Content.Shared._ECHO.Posing;
using Content.Shared.Input;
using Robust.Client.GameObjects;
using Robust.Client.Input;
using Robust.Client.Player;

namespace Content.Client._ECHO.Posing;

public sealed partial class PosingSystem : SharedPosingSystem
{
    [Dependency] private IInputManager _input = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private SpriteSystem _sprite = default!;

    private const float OffsetChangeSpeed = 1f;
    private const float RotationChangeSpeed = 15f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PosingComponent, AfterAutoHandleStateEvent>(OnAfterHandleState);

        // Inherit from human so regular gameplay hotkeys (e.g. ActivateItemInHand on Z)
        // remain available while posing mode is active.
        var posing = _input.Contexts.New("posing", "human");
        posing.AddFunction(ContentKeyFunctions.TogglePosing);
        posing.AddFunction(ContentKeyFunctions.PosingOffsetUp);
        posing.AddFunction(ContentKeyFunctions.PosingOffsetDown);
        posing.AddFunction(ContentKeyFunctions.PosingOffsetLeft);
        posing.AddFunction(ContentKeyFunctions.PosingOffsetRight);
        posing.AddFunction(ContentKeyFunctions.PosingRotatePositive);
        posing.AddFunction(ContentKeyFunctions.PosingRotateNegative);
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _input.Contexts.Remove("posing");
    }

    private void OnAfterHandleState(EntityUid uid, PosingComponent component, ref AfterAutoHandleStateEvent args)
    {
        if (_playerManager.LocalEntity == uid)
            return;

        if (!component.Posing)
        {
            component.TargetOffset = component.DefaultOffset;
            component.TargetAngle = component.DefaultAngle;
            return;
        }
    }

    protected override void ClientTogglePosing(EntityUid uid, PosingComponent posing)
    {
        base.ClientTogglePosing(uid, posing);

        _input.Contexts.SetActiveContext(posing.Posing ? "posing" : posing.DefaultInputContext);

        if (!posing.Posing)
        {
            posing.TargetOffset = posing.DefaultOffset;
            posing.TargetAngle = posing.DefaultAngle;
        }
    }

    // возможно не самый лучший способ, но вы б знали, как я не хочу возиться со всеми остальными анимациями
    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        var query = EntityQueryEnumerator<PosingComponent>();
        while (query.MoveNext(out var uid, out var posing))
        {
            posing.CurrentOffset = VectorExtensions.MoveTowards(posing.CurrentOffset, posing.DefaultOffset + posing.TargetOffset, frameTime * OffsetChangeSpeed);
            posing.CurrentAngle = AngleExtensions.MoveTowards(posing.CurrentAngle, posing.TargetAngle, frameTime * RotationChangeSpeed);

            _sprite.SetOffset(uid, posing.CurrentOffset);
            _sprite.SetRotation(uid, posing.CurrentAngle);
        }
    }
}
