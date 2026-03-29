using System.Numerics;
using Content.Shared._ECHO.Tools;
using Content.Shared.DoAfter;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;

namespace Content.Client._ECHO.Tools;

public sealed class WeldingSparksSystem : SharedWeldingSparksSystem
{
    [Dependency] private readonly AnimationPlayerSystem _animation = default!;
    [Dependency] private readonly IEyeManager _eyeManager = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private const string ANIM_KEY = "WeldAnim";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<SpawnWeldingSparksEvent>(OnSpawnWeldingSparks);
        SubscribeNetworkEvent<StopWeldingSparksEvent>(OnStopWeldingSparks);
    }

    private void OnSpawnWeldingSparks(SpawnWeldingSparksEvent ev)
    {
        if (!TryGetEntity(ev.TargetEnt, out var targetEnt) ||
            !TryGetEntity(ev.Tool, out var tool))
            return;

        Spawn(tool.Value, targetEnt.Value, GetCoordinates(ev.TargetPos), ev.DoAfterIdx, ev.Duration);
    }

    private void OnStopWeldingSparks(StopWeldingSparksEvent ev)
    {
        if (!TryGetEntity(ev.Tool, out var tool))
            return;

        if (!TryComp<WeldingSparksComponent>(tool, out var sparks))
            return;

        StopEffect((tool.Value, sparks), tool.Value, ev.DoAfterIdx);
    }

    private void Spawn(EntityUid tool, EntityUid targetEnt, EntityCoordinates targetPos, ushort doAfterIdx, TimeSpan duration)
    {
        if (!TryComp<WeldingSparksComponent>(tool, out var sparks))
            return;

        if (!TryComp<WeldableComponent>(targetEnt, out var weldableComp) ||
            !TryComp<WeldingSparksAnimationComponent>(targetEnt, out var sparksAnim))
            return;

        var sparksEnt = Spawn(sparks.EffectProto, targetPos);
        sparks.SpawnedEffects.Add(doAfterIdx, sparksEnt);

        EnsureComp<TimedDespawnComponent>(sparksEnt).Lifetime = (float)duration.TotalSeconds + .25f;

        var animationPlayer = EnsureComp<AnimationPlayerComponent>(targetEnt);
        if (_animation.HasRunningAnimation(targetEnt, animationPlayer, ANIM_KEY))
            return;

        var (startOffset, endOffset) = GetOffsets((targetEnt, sparksAnim), weldableComp.IsWelded);

        var animation = new Animation()
        {
            Length = duration,
            AnimationTracks =
            {
                new AnimationTrackComponentProperty()
                {
                    ComponentType = typeof(SpriteComponent),
                    Property = nameof(SpriteComponent.Offset),
                    InterpolationMode = AnimationInterpolationMode.Linear,
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(startOffset, 0f),
                        new AnimationTrackProperty.KeyFrame(endOffset, (float) duration.TotalSeconds),
                    }
                }
            }
        };

        _animation.Play(sparksEnt, animation, ANIM_KEY);
    }

    private (Vector2, Vector2) GetOffsets(Entity<WeldingSparksAnimationComponent> ent, bool isWelded)
    {
        var start = ent.Comp.StartingOffset;
        // If there's no manual `EndingOffset`, just go to the opposite of `StartingOffset`.
        var end = ent.Comp.EndingOffset ?? -ent.Comp.StartingOffset;

        // Rotation
        // Honestly I don't understand all of RT's sprite/eye/world/cardinal rotation stuff. I just trial-and-error'd this into working.
        // (why isn't there a helper function for this) :(
        if (TryComp<SpriteComponent>(ent, out var sprite))
        {
            var worldRotation = _transformSystem.GetWorldRotation(ent);
            var eyeRotation = _eyeManager.CurrentEye.Rotation;

            var relativeRotation = (worldRotation + eyeRotation).Reduced().FlipPositive();

            var cardinalSnapping = sprite.SnapCardinals ? relativeRotation.GetCardinalDir().ToAngle() : Angle.Zero;

            var finalAngle = sprite.NoRotation ? relativeRotation : relativeRotation - cardinalSnapping;

            start = finalAngle.RotateVec(start); // `RotateVec()` contains a `Theta == 0` check, so no need to check for it in here.
            end = finalAngle.RotateVec(end);
        }

        // Welding.
        if (!isWelded)
        {
            return (start, end);
        }
        // Unwelding. (go backwards)
        else
        {
            return (end, start);
        }
    }

    protected override void DoEffect(Entity<WeldingSparksComponent> ent, EntityUid user, EntityUid? target, TimeSpan duration, DoAfterId id, EntityCoordinates spawnLoc)
    {
        if (!_timing.IsFirstTimePredicted)
            return;

        if (!target.HasValue)
            return;

        Spawn(ent.Owner, target.Value, spawnLoc, id.Index, duration);
    }

    protected override void StopEffect(Entity<WeldingSparksComponent> ent, EntityUid user, ushort doAfterIdx)
    {
        if (ent.Comp.SpawnedEffects.TryGetValue(doAfterIdx, out var effect))
            QueueDel(effect);
    }
}
