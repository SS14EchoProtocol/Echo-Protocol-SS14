using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using Content.Server._CE.ZLevels.Core;
using Content.Shared._CE.ZLevels.Core.Components;
using Content.Shared._Utopia.ZLevels.Components;
using Content.Shared._Utopia.ZLevels.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.ContentPack;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Utility;

namespace Content.Server._Utopia.ZLevels;

public sealed class ZNetworkMappingSystem : EntitySystem
{
    [Dependency] private readonly IResourceManager _resMan = default!;
    [Dependency] private readonly MapSystem _map = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly CEZLevelsSystem _zLevels = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly SharedGridMotionLinkSystem _motionLink = default!;

    #region Saving
    public bool TrySaveMap(string path, EntityUid target, [NotNullWhen(false)] out string? error)
    {
        error = null;

        if (_resMan.ContentFindFiles(path).Count() > 0)
        {
            error = $"Target directory is not empty.";
            return false;
        }

        if (!TryComp<CEZLevelsNetworkComponent>(target, out var levelComp))
        {
            error = $"Target entity doesnt have CEZLevelsNetworkComponent {target}";
            return false;
        }

        var success = true;
        var data = new SavedZNetworkMapData();
        var i = 0;

        foreach (var (depth, mapUid) in levelComp.ZLevels)
        {
            if (!TryComp<MapComponent>(mapUid, out var mapComp))
            {
                error = $"Map entity {mapUid} doesnt have MapComponent.";
                return false;
            }

            var mapId = mapComp.MapId;

            // no saving null space
            if (mapId == MapId.Nullspace)
                continue;

            if (!_map.MapExists(mapId))
            {
                error = $"Map {mapId} doesnt exist!";
                return false;
            }

            if (_map.IsInitialized(mapId))
            {
                error = $"Map {mapId} is already initialized, cannot save initialized maps!";
                return false;
            }

            data.LevelPaths[i] = $"map-depth-{depth}.yml";
            i++;

            var savePath = new ResPath($"{path}/map-depth-{depth}.yml");

            if (!_mapLoader.TrySaveMap(mapId, savePath))
            {
                success = false;
                break;
            }
        }

        if (!success)
        {
            _resMan.UserData.Delete(new(path));
            error ??= $"Unknkown error occured during map saving.";
            return false;
        }

        var resPath = new ResPath($"{path}/map-data.json");
        var jsonData = JsonSerializer.Serialize<SavedZNetworkMapData>(data, new JsonSerializerOptions(JsonSerializerDefaults.General));
        _resMan.UserData.WriteAllText(resPath, jsonData);

        return true;
    }

    public bool TrySaveGrid(string path, EntityUid target, [NotNullWhen(false)] out string? error)
    {
        error = null;

        if (_resMan.ContentFindFiles(path).Count() > 0)
        {
            error = $"Target directory is not empty.";
            return false;
        }

        if (!TryComp<GridMotionLinkComponent>(target, out var link))
        {
            error = $"Target entity doesnt have GridMotionLinkComponent {target}";
            return false;
        }

        if (Transform(target).MapUid is not { Valid: true } mapUid)
        {
            error = $"Target map is not valid.";
            return false;
        }

        if (!_zLevels.TryGetZNetwork(mapUid, out var net))
        {
            error = $"Can not find ZNetwork for target map";
            return false;
        }

        var data = new SavedZNetworkGridData();
        var (pos, rotation) = _transform.GetWorldPositionRotation(target);

        // There we go through all levels to get all linked grids
        var levels = net.Value.Comp.ZLevels.OrderBy(x => x.Key).ToList();
        for (var i = 0; i < levels.Count(); i++)
        {
            if (levels[i].Value is not { Valid: true } levelMap)
                continue;

            if (!SaveLevelGrids(levelMap, data, path, link.GroupId, pos, rotation, i))
            {
                _resMan.UserData.Delete(new(path));
                error ??= $"Unknkown error occured during grid saving.";
                return false;
            }
        }

        var resPath = new ResPath($"{path}/grid-data.json");
        var jsonData = JsonSerializer.Serialize<SavedZNetworkGridData>(data, new JsonSerializerOptions(JsonSerializerDefaults.General));
        _resMan.UserData.WriteAllText(resPath, jsonData);

        return true;
    }

    /// <summary>
    /// Saves all grids on target ZLevel
    /// </summary>
    /// <param name="levelMap">Level map uid.</param>
    /// <param name="data"><see cref="SavedZNetworkGridData"/> where all grids would be saved.</param>
    /// <param name="path">Save folder path.</param>
    /// <param name="groupId">Grid motion link group that will be saved.</param>
    /// <param name="pos">World position of center that all other grids will be related to.</param>
    /// <param name="depth">Level depth</param>
    /// <returns></returns>
    private bool SaveLevelGrids(EntityUid levelMap, SavedZNetworkGridData data, string path, string groupId, Vector2 pos, Angle rotation, int depth)
    {
        var grids = EntityManager.AllEntities<GridMotionLinkComponent>()
                                     .Where(x => Transform(x.Owner).MapUid == levelMap && x.Comp.GroupId == groupId);

        data.LevelPaths[depth] = new();
        var j = 0;
        foreach (var item in grids)
        {
            var fileName = $"map-depth-{depth}-grid-{j}.yml";

            var gridPos = _transform.GetWorldPosition(item.Owner);
            Quaternion2D q = new(rotation);
            var gridOffset = Quaternion2D.RotateVector(q, gridPos - pos);

            // Save grid to data
            var gridLevel = new SavedGridLevel(fileName, gridOffset);
            data.LevelPaths[depth].Add(gridLevel);

            // Save grid itself
            if (!_mapLoader.TrySaveGrid(item.Owner, new($"{path}/{fileName}")))
                return false;
        }

        return true;
    }
    #endregion

    #region Loading
    public bool TryLoadMap(string path,
                           [NotNullWhen(true)] out List<Entity<MapComponent>>? maps,
                           [NotNullWhen(true)] out List<Entity<MapGridComponent>>? grids,
                           [NotNullWhen(false)] out string? error)
    {
        var id = new MapId((int)_map.GetAllMapIds().Max() + 1);
        return TryLoadMap(path, id, out maps, out grids, out error);
    }

    public bool TryLoadMap(ResPath path,
                           [NotNullWhen(true)] out List<Entity<MapComponent>>? maps,
                           [NotNullWhen(true)] out List<Entity<MapGridComponent>>? grids,
                           [NotNullWhen(false)] out string? error)
    {
        var id = new MapId((int)_map.GetAllMapIds().Max() + 1);
        return TryLoadMap(path, id, out maps, out grids, out error);
    }

    public bool TryLoadMap(string path, MapId mapId,
                           [NotNullWhen(true)] out List<Entity<MapComponent>>? maps,
                           [NotNullWhen(true)] out List<Entity<MapGridComponent>>? grids,
                           [NotNullWhen(false)] out string? error)
    {
        return TryLoadMap(new ResPath(path), mapId, out maps, out grids, out error);
    }

    public bool TryLoadMap(ResPath path, MapId mapId,
                           [NotNullWhen(true)] out List<Entity<MapComponent>>? maps,
                           [NotNullWhen(true)] out List<Entity<MapGridComponent>>? grids,
                           [NotNullWhen(false)] out string? error)
    {
        error = null;
        maps = null;
        grids = null;

        if (_map.TryGetMap(mapId, out var mapUid))
        {
            error = "Target map already exsists.";
            return false;
        }

        if (!_resMan.UserData.TryReadAllText(path, out var rawData))
        {
            error = "Could not find map-data.json file in target directory.";
            return false;
        }

        var data = JsonSerializer.Deserialize<SavedZNetworkMapData>(rawData);

        if (data == null)
        {
            error = "Error occured when reading map-data.json";
            return false;
        }

        for (var m = 0; m > data.LevelPaths.Count; m++)
        {
            if (_map.TryGetMap(new((int)mapId + m), out _))
            {
                error = $"Upper map {(int)mapId + m} already exsists.";
                return false;
            }
        }

        return LoadMap(path.CanonPath, mapId, data, out maps, out grids);
    }

    public bool TryLoadGrid(string path, MapId mapId, Vector2 offset, float rotation, [NotNullWhen(false)] out string? error)
    {
        error = null;

        var dataPath = new ResPath($"{path}/grid-data.json").ToRootedPath();
        if (!_resMan.UserData.TryReadAllText(dataPath, out var rawData))
        {
            error = "Could not find grid-data.json file in target directory.";
            return false;
        }

        var data = JsonSerializer.Deserialize<SavedZNetworkGridData>(rawData);

        if (data == null)
        {
            error = "Error occured when reading grid-data.json";
            return false;
        }

        var angle = Angle.FromDegrees(rotation);

        if (!_map.TryGetMap(mapId, out var mapUid))
        {
            return LoadGridWithMaps(path, data, mapId, offset, angle);
        }
        else
        {
            return LoadGridOnMap(path, mapId, mapUid.Value, data, offset, angle);
        }
    }

    private bool LoadGridWithMaps(string path, SavedZNetworkGridData data, MapId mapId, Vector2 offset, Angle rotation)
    {
        var network = _zLevels.CreateZNetwork();
        Dictionary<EntityUid, int> maps = new();

        for (var i = 0; i < data.LevelPaths.Count; i++)
        {
            var item = data.LevelPaths[i];

            var map = i == 0 ? _map.CreateMap(mapId, false) : _map.CreateMap(false);
            maps.Add(map, i);

            foreach (var grid in item)
            {
                var mapPath = new ResPath($"{path}/{grid.Path}");

                Quaternion2D q = new(rotation);
                var gridOffset = Quaternion2D.RotateVector(q, grid.Offset);

                _mapLoader.TryLoadGrid(Comp<MapComponent>(map).MapId, mapPath, out _, offset: offset + gridOffset, rot: rotation);
            }
        }

        return _zLevels.TryAddMapsIntoZNetwork(network, maps);
    }

    private bool LoadMap(string path, MapId mapId, SavedZNetworkMapData data,
                         [NotNullWhen(true)] out List<Entity<MapComponent>>? resultMaps,
                         [NotNullWhen(true)] out List<Entity<MapGridComponent>>? resultGrids)
    {
        resultMaps = new();
        resultGrids = new();

        var network = _zLevels.CreateZNetwork();
        Dictionary<EntityUid, int> maps = new();
        var i = 0;

        foreach (var item in data.LevelPaths.OrderBy(x => x.Key))
        {
            var mapPath = new ResPath($"{path}/{item.Value}");
            var curMapId = new MapId((int)mapId + i);
            if (_mapLoader.TryLoadMapWithId(curMapId, mapPath, out var map, out var grids))
            {
                maps.Add(map.Value.Owner, i);
                resultMaps.Add(map.Value);
                resultGrids.AddRange(grids);
            }

            i++;
        }
        if (!_zLevels.TryAddMapsIntoZNetwork(network, maps))
            return false;

        var ents = EntityManager.AllEntities<GridMotionLinkComponent>().Where(x => maps.ContainsKey(Transform(x.Owner).MapUid ?? EntityUid.Invalid));
        foreach (var linked in ents)
            _motionLink.UpdateOffset(linked);
        foreach (var linked in ents)
            Dirty(linked);

        return true;
    }

    private bool LoadGridOnMap(string path, MapId mapId, EntityUid mapUid, SavedZNetworkGridData data, Vector2 offset, Angle rotation)
    {
        if (!_zLevels.TryGetZNetwork(mapUid, out var network))
            network = _zLevels.CreateZNetwork();

        if (!_map.MapExists(mapId))
            return false;

        _zLevels.TryAddMapsIntoZNetwork(network.Value, new() { { mapUid, 0 } });

        var levels = network.Value.Comp.ZLevels;

        int mapHeight = 0;
        foreach (var item in levels)
        {
            if (item.Value == mapUid)
                mapHeight = item.Key;
        }

        var addedMaps = new Dictionary<EntityUid, int>();
        var linkedGrids = new List<Entity<GridMotionLinkComponent>>();

        for (var i = 0; i < data.LevelPaths.Count; i++)
        {
            var item = data.LevelPaths.ElementAt(i);
            var targetLevel = i + mapHeight;

            if (!levels.TryGetValue(targetLevel, out var map))
            {
                map = _map.CreateMap(_map.IsInitialized(mapId));
                addedMaps.Add(map.Value, targetLevel);
            }

            var mapComp = Comp<MapComponent>(map!.Value);

            foreach (var grid in item.Value)
            {
                Quaternion2D q = new(rotation);
                var gridOffset = Quaternion2D.RotateVector(q, grid.Offset);

                _mapLoader.TryLoadGrid(mapComp.MapId, new ResPath($"{path}/{grid.Path}"), out var loaded, offset: gridOffset + offset, rot: rotation);

                if (TryComp<GridMotionLinkComponent>(loaded, out var link))
                {
                    link.Root = EntityUid.Invalid;
                    linkedGrids.Add((loaded.Value, link));
                }
            }
        }

        if (!_zLevels.TryAddMapsIntoZNetwork(network.Value, addedMaps))
            return false;

        foreach (var linked in linkedGrids)
            _motionLink.UpdateOffset(linked);
        foreach (var linked in linkedGrids)
            Dirty(linked);

        return true;
    }
    #endregion
}
