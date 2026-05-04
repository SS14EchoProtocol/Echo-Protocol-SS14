using System.Numerics;
using Content.Shared._CE.ZLevels.Core.Components;
using Content.Shared.CCVar;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;

namespace Content.Shared._CE.ZLevels.Core.EntitySystems;

public abstract partial class CESharedZLevelsSystem
{
    private void UpdateMovement(EntityUid uid,
                                CEZPhysicsComponent zPhys,
                                TransformComponent xform,
                                PhysicsComponent physics,
                                float frameTime)
    {
        var oldVelocity = zPhys.Velocity;
        var oldHeight = zPhys.LocalPosition;

        if (physics.BodyStatus == BodyStatus.OnGround)
        {
            //Velocity application
            var velocityEv = new CEGetZVelocityEvent((uid, zPhys));
            RaiseLocalEvent(uid, velocityEv);

            zPhys.Velocity += velocityEv.VelocityDelta * frameTime;
        }

        //Movement application
        zPhys.LocalPosition += zPhys.Velocity * frameTime;
        zPhys.Velocity = Math.Clamp(zPhys.Velocity, -ZVelocityLimit, ZVelocityLimit);

        UpdateGrounded(uid, zPhys, out var landed);
        HandleLevelChange(uid, zPhys);

        if (landed) //Just landed
            HandleFalling(uid, zPhys);

        if (Math.Abs(oldVelocity - zPhys.Velocity) > 0.01f)
            DirtyField(uid, zPhys, nameof(CEZPhysicsComponent.Velocity));

        if (Math.Abs(oldHeight - zPhys.LocalPosition) > 0.01f)
            DirtyField(uid, zPhys, nameof(CEZPhysicsComponent.LocalPosition));
    }

    private void UpdateGrounded(EntityUid uid, CEZPhysicsComponent zPhys, out bool landed)
    {
        landed = false;

        var distanceToGround = zPhys.LocalPosition - zPhys.CurrentGroundHeight;
        var currentlyGrounded = (distanceToGround <= 0.05f || zPhys.CurrentStickyGround) && distanceToGround <= MaxStepHeight;

        if (currentlyGrounded)
        {
            zPhys.LocalPosition -= distanceToGround; //Sticky move
        }

        if (currentlyGrounded == zPhys.IsGrounded)
            return;

        landed = !zPhys.IsGrounded && currentlyGrounded;

        zPhys.IsGrounded = currentlyGrounded;

        if (currentlyGrounded != zPhys.IsGrounded)
            DirtyField(uid, zPhys, nameof(CEZPhysicsComponent.IsGrounded));
    }

    private void HandleFalling(EntityUid uid, CEZPhysicsComponent zPhys)
    {
        if (MathF.Abs(zPhys.Velocity) >= Cfg.GetCVar(EchoCCVars.ZImpactVelocityLimit))
        {
            _queuedLandings.Add(uid, -zPhys.Velocity);
        }

        zPhys.Velocity = -zPhys.Velocity * zPhys.Bounciness;
    }

    private void HandleLevelChange(EntityUid uid, CEZPhysicsComponent zPhys)
    {
        if (zPhys.LocalPosition < 0) //Need teleport to ZLevel down
        {
            if (!TryMoveDownOrChasm(uid))
                return;

            zPhys.LocalPosition += 1;

            if (zPhys.CurrentStickyGround)
                return;

            var fallEv = new CEZLevelFallMapEvent();
            RaiseLocalEvent(uid, fallEv);
        }

        else if (zPhys.LocalPosition >= 1) //Need teleport to ZLevel up
        {
            if (HasTileAbove(uid)) //Hit roof
            {
                if (MathF.Abs(zPhys.Velocity) >= Cfg.GetCVar(EchoCCVars.ZImpactVelocityLimit)) // ECHO-Tweak: перенос констант в конфиг
                {
                    _queuedLandings.Add(uid, zPhys.Velocity);
                }

                zPhys.LocalPosition = 1;
                zPhys.Velocity = -zPhys.Velocity * zPhys.Bounciness;
            }
            else //Move up
            {
                if (TryMoveUp(uid))
                    zPhys.LocalPosition -= 1;
            }
        }
    }

    /// <summary>
    /// Computes the "ground height" relative to the entity's current Z-level baseline.
    /// Returns values where 0 means ground on the same level, -1 means ground one level below,
    /// and intermediate values are possible for high ground entities (stairs).
    /// </summary>
    private float ComputeGroundHeightInternal(Entity<CEZPhysicsComponent?> target, out bool stickyGround, int maxFloors = 1)
    {
        stickyGround = false;
        if (!Resolve(target, ref target.Comp, false))
            return 0;

        var xform = Transform(target);
        if (!_zMapQuery.TryComp(xform.MapUid, out var zMapComp))
            return 0;
        if (!_gridQuery.TryComp(xform.MapUid, out var mapGrid))
            return 0;

        var worldPosI = _transform.GetGridOrMapTilePosition(target);
        var worldPos = _transform.GetWorldPosition(target);

        //Select current map by default
        Entity<CEZLevelMapComponent> checkingMap = (xform.MapUid.Value, zMapComp);
        var checkingGrid = mapGrid;

        for (var floor = 0; floor <= maxFloors; floor++)
        {
            if (floor != 0) //Select map below
            {
                if (!TryMapOffset((checkingMap.Owner, checkingMap.Comp), -floor, out var tempCheckingMap))
                    continue;
                if (!_gridQuery.TryComp(tempCheckingMap, out var tempCheckingGrid))
                    continue;

                checkingMap = tempCheckingMap.Value;
                checkingGrid = tempCheckingGrid;
            }

            //Check all types of ZHeight entities
            var query = _map.GetAnchoredEntitiesEnumerator(checkingMap, checkingGrid, worldPosI);
            while (query.MoveNext(out var ent))
            {
                if (!_highgroundQuery.TryComp(ent, out var heightComp))
                    continue;

                var uid = ent.Value;

                var fix = _fix.GetFixtureOrNull(uid, heightComp.FixtureId);

                if (fix == null || fix.Shape is not PolygonShape shape)
                    continue;


                var transform = new Transform(0f);
                var bottom = shape.ComputeAABB(transform, 0).Bottom;
                var top = shape.ComputeAABB(transform, 0).Top;
                var length = Math.Abs(top - bottom);

                var (pos, rot) = _transform.GetWorldPositionRotation(uid);

                var bottomPos = rot.RotateVec(new Vector2(0, bottom)) + pos;

                var curve = heightComp.HeightCurve;
                if (curve.Count == 0)
                    continue;

                if (curve.Count == 1)
                {
                    var groundY = curve[0];
                    // groundHeight is negative downwards: -floor + groundY
                    return -floor + groundY;
                }

                // Calculate the world direction of the fixture (assuming it's along local Y)
                var worldDir = rot.RotateVec(new Vector2(0, length));
                var lengthWorld = worldDir.Length();
                if (lengthWorld == 0)
                    continue;

                stickyGround = heightComp.Stick;

                // Project the entity's position onto the fixture line
                var relPos = worldPos - bottomPos;
                var t = Vector2.Dot(relPos, worldDir) / (lengthWorld * lengthWorld); // Dot with normalized dir
                t = Math.Clamp(t, 0f, 1f);

                // Invert t to match curve ordering (curve[0] should represent the *bottom* of the fixture).
                t = 1f - t;

                // Interpolate in the height curve
                float index = t * (curve.Count - 1);
                int lower = (int)Math.Floor(index);
                int upper = Math.Min(lower + 1, curve.Count - 1);
                float frac = index - lower;
                var y = curve[lower] * (1 - frac) + curve[upper] * frac;

                // groundHeight is negative downwards: -floor + y
                return -floor + y;
            }

            //No ZEntities found, check floor tiles
            if (_map.TryGetTileRef(checkingMap, checkingGrid, worldPosI, out var tileRef) &&
                !tileRef.Tile.IsEmpty)
                return -floor; // tile ground has groundY == 0 -> -floor
        }

        return -maxFloors;
    }
}
