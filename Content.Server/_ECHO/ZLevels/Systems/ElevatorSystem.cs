using System.Linq;
using Content.Server.GameTicking;
using Content.Shared._Echo.ZLevels;
using Robust.Server.GameObjects;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Server._Echo.ZLevels;

public sealed class ElevatorSystem : EntitySystem
{
    [Dependency] private readonly TransformSystem _xform = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    private Dictionary<string, EntityUid> _elevators = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PostGameMapLoad>(OnGameMapLoad);
    }

    private void OnGameMapLoad(PostGameMapLoad args)
    {
        SetupElevator(args.Map);
        UpdateUi();
    }

    private void UpdateUi()
    {
        var elevators = EntityManager.AllEntities<ElevatorComponent>();

        foreach (var elevator in elevators)
        {
            UpdateUi(elevator);
        }
    }

    private void UpdateUi(Entity<ElevatorComponent> elevator)
    {
        var points = EntityManager.AllEntities<ElevatorPointComponent>();
        var controllers = EntityManager.AllEntities<ElevatorControllerComponent>();
        List<ElevatorFloorData> data = new();

        foreach (var item in points.Where(x => x.Comp.Group == elevator.Comp.Group))
        {
            data.Add(item.Comp.FloorData);
        }

        var state = new ElevatorControllerUiState(data, elevator.Comp.InProgress);
        var childEnumerator = Transform(elevator).ChildEnumerator;

        while (childEnumerator.MoveNext(out var uid))
        {
            if (!TryComp<ElevatorControllerComponent>(uid, out var controller))
                continue;

            _ui.SetUiState(uid, ElevatorControllerUiKey.Key, state);
        }
    }

    public void SetupElevator(MapId mapId)
    {
        var points = EntityManager.AllEntities<ElevatorPointComponent>();
        var unusedPoints = points.ToList();

        foreach (var point in points)
        {
            if (Transform(point).MapID != mapId)
                continue;

            if (_elevators.ContainsKey(point.Comp.Group) || TryLoadElevator(mapId, point))
                unusedPoints.Remove(point);
        }

        foreach (var point in unusedPoints)
        {
            TryLoadElevator(Transform(point).MapID, point);
        }
    }

    private bool TryLoadElevator(MapId mapId, Entity<ElevatorPointComponent> point)
    {
        var pos = _xform.GetWorldPosition(point.Owner);

        if (!_mapLoader.TryLoadGrid(mapId, new ResPath(point.Comp.GridPath).ToRootedPath(), out var grid, offset: pos + point.Comp.Offset))
            return false;

        _elevators.Add(point.Comp.Group, grid.Value.Owner);

        var comp = EnsureComp<ElevatorComponent>(grid.Value.Owner);
        comp.Group = point.Comp.Group;

        UpdateUi((grid.Value.Owner, comp));

        return true;
    }
}
