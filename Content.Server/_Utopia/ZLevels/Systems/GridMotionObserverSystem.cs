using Content.Server._Utopia.ZLevels.Components;
using Content.Server._Utopia.ZLevels.Events;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using Robust.Shared.Maths;
using System.Numerics;

namespace Content.Server._Utopia.ZLevels.Systems;

public sealed class GridMotionObserverSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    private const float Epsilon = 0.0001f;

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<GridMotionObserverComponent, PhysicsComponent>();

        while (query.MoveNext(out var uid, out var observer, out var physics))
        {
            if (observer.SuppressNextTick)
            {
                observer.SuppressNextTick = false;
                continue;
            }

            var lin = physics.LinearVelocity;
            var ang = physics.AngularVelocity;

            if ((lin - observer.LastLinearVelocity).LengthSquared() < Epsilon &&
                MathF.Abs(ang - observer.LastAngularVelocity) < Epsilon)
                continue;

            observer.LastLinearVelocity = lin;
            observer.LastAngularVelocity = ang;

            var dir = lin.LengthSquared() > Epsilon ? lin.Normalized() : Vector2.Zero;

            RaiseLocalEvent(uid, new GridMotionChangedEvent
            {
                LinearDirection = dir,
                LinearPower = lin.Length(),
                AngularPower = ang,
                Time = _timing.CurTime
            });
        }
    }
}
