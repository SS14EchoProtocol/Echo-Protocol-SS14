using Content.Shared._Utopia.ZLevels.Components;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Content.Shared._Utopia.ZLevels.Systems;

public abstract class SharedGridMotionLinkSystem : EntitySystem
{
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;

    private const string GlobalGroupId = "ZZZ";

    public override void Initialize()
    {
        base.Initialize();

        UpdatesAfter.Add(typeof(SharedPhysicsSystem));
    }


    public void UpdateOffset(Entity<GridMotionLinkComponent> ent)
    {
        ent.Comp.Root = GetBiggestGridOfGroup(ent.Comp.GroupId);

        if (ent.Comp.Root is not { Valid: true })
            ent.Comp.Root = ent.Owner;

        if (ent.Comp.Root == ent.Owner)
            ent.Comp.Offset = Vector2.Zero;
        else
        {
            var rootRot = _transformSystem.GetWorldRotation(ent.Comp.Root);
            var rootPos = _transformSystem.GetWorldPosition(ent.Comp.Root);
            var entPos = _transformSystem.GetWorldPosition(ent.Owner);

            var q = new Quaternion2D(rootRot);
            ent.Comp.Offset = Quaternion2D.InvRotateVector(q, entPos - rootPos);

            _transformSystem.SetWorldRotation(ent.Owner, rootRot);
        }

        Dirty(ent);
    }

    public void InitializeGrid(EntityUid gridUid)
    {
        var link = EnsureComp<GridMotionLinkComponent>(gridUid);
        link.GroupId = GlobalGroupId;
    }

    private void RelayMotion(Vector2 linear, float angular,
                             EntityUid biggest,
                             List<Entity<GridMotionLinkComponent, MapGridComponent, PhysicsComponent>> matches)
    {
        foreach (var (targetUid, link, grid, phys) in matches)
        {
            _physics.SetLinearVelocity(targetUid, linear, body: phys);
            _physics.SetAngularVelocity(targetUid, angular, body: phys);

            if (biggest != targetUid)
                SetOffsetPos(biggest, (targetUid, link));
        }
    }

    private void SetOffsetPos(EntityUid biggest, Entity<GridMotionLinkComponent> ent)
    {
        var rootRot = _transformSystem.GetWorldRotation(biggest);
        var rootPos = _transformSystem.GetWorldPosition(biggest);

        var q = new Quaternion2D(rootRot);
        var newPos = rootPos + Quaternion2D.RotateVector(q, ent.Comp.Offset);

        _transformSystem.SetWorldPosition(ent.Owner, newPos);
        _transformSystem.SetWorldRotation(ent.Owner, rootRot);
        _physics.WakeBody(ent.Owner);
    }

    private bool TryGetMotionData(EntityUid uid,
                                 [NotNullWhen(true)] out Vector2? linearSpeed,
                                 [NotNullWhen(true)] out float? angularSpeed,
                                 [NotNullWhen(true)] out EntityUid? biggestGrid,
                                  out List<Entity<GridMotionLinkComponent, MapGridComponent, PhysicsComponent>> matches,
                                  GridMotionLinkComponent? comp = null)
    {
        linearSpeed = Vector2.Zero;
        angularSpeed = 0f;
        biggestGrid = null;
        matches = new();

        if (!Resolve(uid, ref comp))
            return false;

        matches = GetGridsOfGroup(comp.GroupId);

        if (matches.Count == 0)
            return false;


        var biggest = new KeyValuePair<int, EntityUid>(0, EntityUid.Invalid);
        foreach (var (targetUid, link, grid, phys) in matches)
        {
            if (link.GroupId != comp.GroupId)
                continue;

            linearSpeed += phys.LinearVelocity;
            angularSpeed += phys.AngularVelocity;

            var tilesCount = _map.GetAllTiles(targetUid, grid, true).Count();

            if (biggest.Key < tilesCount)
                biggest = new(tilesCount, targetUid);
        }

        linearSpeed /= matches.Count;
        angularSpeed /= matches.Count;
        biggestGrid = biggest.Value;
        return true;
    }

    private List<Entity<GridMotionLinkComponent, MapGridComponent, PhysicsComponent>> GetGridsOfGroup(string groupId)
    {
        var matches = new List<Entity<GridMotionLinkComponent, MapGridComponent, PhysicsComponent>>();

        var query = EntityQueryEnumerator<GridMotionLinkComponent, MapGridComponent, PhysicsComponent>();
        while (query.MoveNext(out var targetUid, out var link, out var grid, out var phys))
        {
            if (link.GroupId != groupId)
                continue;

            matches.Add((targetUid, link, grid, phys));
        }

        return matches;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<GridMotionLinkComponent, PhysicsComponent>();
        List<string> groupsMoved = new();

        while (query.MoveNext(out var uid, out var comp, out var phys))
        {
            if (groupsMoved.Contains(comp.GroupId))
                continue;

            if (!TryGetMotionData(uid, out var linear, out var angular, out var biggest, out var group, comp))
                continue;

            RelayMotion(linear.Value, angular.Value, biggest.Value, group);
            groupsMoved.Add(comp.GroupId);
        }
    }

    public void SetGridPosition(EntityUid origin, Vector2 position, GridMotionLinkComponent? link = null)
    {
        if (!Resolve(origin, ref link, false))
        {
            _transformSystem.SetWorldPosition(origin, position);
            return;
        }

        var diff = position - _transformSystem.GetWorldPosition(origin);

        var query = EntityQueryEnumerator<GridMotionLinkComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (link.GroupId != comp.GroupId)
                continue;

            var targetPos = _transformSystem.GetWorldPosition(uid) + diff;
            _transformSystem.SetWorldPosition(uid, targetPos);
            _physics.SetLinearVelocity(uid, Vector2.Zero);
        }
    }

    private EntityUid GetBiggestGridOfGroup(string group)
    {
        var ents = EntityManager.AllEntities<GridMotionLinkComponent>().Where(x => x.Comp.GroupId == group);

        var biggest = new KeyValuePair<int, EntityUid>(0, EntityUid.Invalid);
        foreach (var ent in ents)
        {
            if (!TryComp<MapGridComponent>(ent.Owner, out var grid))
                continue;

            var tilesCount = _map.GetAllTiles(ent.Owner, grid, true).Count();

            if (biggest.Key < tilesCount)
                biggest = new(tilesCount, ent.Owner);
        }

        return biggest.Value;
    }
}
