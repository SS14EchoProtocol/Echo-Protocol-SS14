using Content.Server._Utopia.ZLevels.Pipes.Systems;
using Content.Server._Utopia.ZLevels.Nodes;
using Content.Server._Utopia.ZLevels.Power;
using Content.Shared._Utopia.ZLevels.Cables.Components;
using Content.Server.Disposal.Tube;
using Content.Server._Utopia.ZLevels.Disposal.Components;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.NodeContainer;
using Content.Shared._CE.ZLevels.Core.Components;
using Content.Shared._CE.ZLevels.Core.EntitySystems;
using Content.Shared._Utopia.ZLevels.Pipes.Components;
using Content.Shared._Utopia.ZLevels.Transmission.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using System.Numerics;

namespace Content.Server._Utopia.ZLevels.Transmission.Systems;

public sealed class ZLevelTransmissionSystem : EntitySystem
{
    [Dependency] private readonly CESharedZLevelsSystem _zLevels = default!;
    [Dependency] private readonly ZPipeSystem _zPipes = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly ZCableSystem _zCables = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ZLevelTransmitterComponent, ComponentStartup>(OnRefresh);
        SubscribeLocalEvent<ZLevelTransmitterComponent, MoveEvent>(OnMove);
    }

    private void OnRefresh(EntityUid uid, ZLevelTransmitterComponent comp, ComponentStartup args)
        => Refresh(uid, comp);

    private void OnMove(EntityUid uid, ZLevelTransmitterComponent comp, ref MoveEvent args)
        => Refresh(uid, comp);

    #region Refresh
    private void Refresh(EntityUid uid, ZLevelTransmitterComponent transmitter)
    {
        if (!TryGetContext(uid, out var ctx))
            return;

        var link = EnsureComp<ZLevelEntityLinkComponent>(uid);

        link.ZNetwork = ctx.ZNetwork;
        link.Depth = ctx.ZMap.Depth;
        link.MapEntity = ctx.MapUid;
        link.GridEntity = transmitter.UseGrid ? ctx.Transform.GridUid : null;

        link.AboveMap = transmitter.AllowUp ? GetNeighborMap(ctx, 1) : null;
        link.BelowMap = transmitter.AllowDown ? GetNeighborMap(ctx, -1) : null;

        if (HasComp<ZPipeComponent>(uid))
            RebuildPipeLinks(uid, link, transmitter);

        if (HasComp<ZCableComponent>(uid))
            RebuildCableLinks(uid, link, transmitter);
    }
    #endregion

    #region Pipes
    private void RebuildPipeLinks(EntityUid uid, ZLevelEntityLinkComponent link, ZLevelTransmitterComponent transmitter)
    {
        if (!TryGetAnchoredGrid(uid, out var xform, out var gridUid, out var grid))
            return;

        if (!TryComp(uid, out NodeContainerComponent? container))
            return;

        _zPipes.ClearAll(container);

        var worldBox = GetTileRangeBox(gridUid, grid, xform, transmitter.Range);

        foreach (var node in container.Nodes.Values)
        {
            if (node is not ZPipeNode zNode)
                continue;

            var (targetMap, requiredDir) = ResolveDirection(link, zNode.ZDirection);

            if (targetMap == null)
                continue;

            TryFindPipeMatches(uid, zNode, worldBox, targetMap.Value, requiredDir);
        }
    }

    private void TryFindPipeMatches(
        EntityUid source,
        ZPipeNode self,
        Box2 worldBox,
        EntityUid targetMap,
        ZNodeDirection requiredDir)
    {
        if (!TryComp(targetMap, out TransformComponent? mapXform))
            return;

        foreach (var ent in _lookup.GetEntitiesIntersecting(mapXform.MapID, worldBox, LookupFlags.All))
        {
            if (ent == source)
                continue;

            if (!TryComp(ent, out TransformComponent? xform) || !xform.Anchored)
                continue;

            if (!TryComp(ent, out NodeContainerComponent? container))
                continue;

            foreach (var node in container.Nodes.Values)
            {
                if (node is not ZPipeNode other)
                    continue;

                if (other.ZDirection != requiredDir)
                    continue;

                _zPipes.AddZConnection(self, other);
            }
        }
    }
    #endregion

    #region Cables
    private void RebuildCableLinks(EntityUid uid, ZLevelEntityLinkComponent link, ZLevelTransmitterComponent transmitter)
    {
        if (!TryGetAnchoredGrid(uid, out var xform, out var gridUid, out var grid))
            return;

        if (!TryComp(uid, out NodeContainerComponent? container))
            return;

        _zCables.ClearAll(container);

        var worldBox = GetTileRangeBox(gridUid, grid, xform, transmitter.Range);

        foreach (var node in container.Nodes.Values)
        {
            if (node is not ZCableNode zNode)
                continue;

            var (targetMap, requiredDir) = ResolveDirection(link, zNode.ZDirection);

            if (targetMap == null)
                continue;

            TryFindCableMatches(uid, zNode, worldBox, targetMap.Value, requiredDir);
        }
    }

    private void TryFindCableMatches(
        EntityUid source,
        ZCableNode self,
        Box2 worldBox,
        EntityUid targetMap,
        ZNodeDirection requiredDir)
    {
        if (!TryComp(targetMap, out TransformComponent? mapXform))
            return;

        foreach (var ent in _lookup.GetEntitiesIntersecting(mapXform.MapID, worldBox, LookupFlags.All))
        {
            if (ent == source)
                continue;

            if (!TryComp(ent, out TransformComponent? xform) || !xform.Anchored)
                continue;

            if (!TryComp(ent, out NodeContainerComponent? container))
                continue;

            foreach (var node in container.Nodes.Values)
            {
                if (node is not ZCableNode other)
                    continue;

                if (other.ZDirection != requiredDir)
                    continue;

                _zCables.AddZConnection(self, other);
            }
        }
    }
    #endregion

    #region Disposal
    public EntityUid? TryFindZDisposalTarget(
        EntityUid source,
        EntityUid targetMap,
        ZNodeDirection dir)
    {
        if (!TryGetAnchoredGrid(source, out var xform, out var gridUid, out var grid))
            return null;

        var worldBox = GetTileBox(gridUid, grid, xform);

        var required = dir == ZNodeDirection.Up
            ? ZNodeDirection.Down
            : ZNodeDirection.Up;

        if (!TryComp(targetMap, out TransformComponent? mapXform))
            return null;

        foreach (var ent in _lookup.GetEntitiesIntersecting(mapXform.MapID, worldBox, LookupFlags.All))
        {
            if (!TryComp(ent, out ZDisposalPipeComponent? zPipe))
                continue;

            if (zPipe.ZDirection != required)
                continue;

            if (!TryComp(ent, out TransformComponent? exform) || !exform.Anchored)
                continue;

            if (!TryComp(ent, out DisposalTubeComponent? disposal))
                continue;

            return ent;
        }

        return null;
    }
    #endregion

    #region Helpers
    private bool TryGetAnchoredGrid(
        EntityUid uid,
        out TransformComponent xform,
        out EntityUid gridUid,
        out MapGridComponent grid)
    {
        xform = default!;
        gridUid = default;
        grid = default!;

        if (!TryComp(uid, out TransformComponent? comp))
            return false;

        xform = comp;

        if (!xform.Anchored)
            return false;

        if (xform.GridUid is not { } g)
            return false;

        if (!TryComp(g, out MapGridComponent? mapGrid))
            return false;

        grid = mapGrid;
        gridUid = g;
        return true;
    }

    private (EntityUid?, ZNodeDirection) ResolveDirection(
        ZLevelEntityLinkComponent link,
        ZNodeDirection dir)
    {
        return dir switch
        {
            ZNodeDirection.Up => (link.AboveMap, ZNodeDirection.Down),
            ZNodeDirection.Down => (link.BelowMap, ZNodeDirection.Up),
            _ => (null, ZNodeDirection.Up)
        };
    }

    private Box2 GetTileBox(
        EntityUid gridUid,
        MapGridComponent grid,
        TransformComponent xform)
    {
        var tile = grid.TileIndicesFor(xform.Coordinates);
        var tileSize = grid.TileSize;

        var world =
            _transform.GetWorldPosition(gridUid) +
            new Vector2(tile.X * tileSize, tile.Y * tileSize);

        return new Box2(world, world + new Vector2(tileSize, tileSize));
    }

    private Box2 GetTileRangeBox(
        EntityUid gridUid,
        MapGridComponent grid,
        TransformComponent xform,
        float range)
    {
        var tile = grid.TileIndicesFor(xform.Coordinates);
        var tileSize = grid.TileSize;

        var origin =
            _transform.GetWorldPosition(gridUid) +
            new Vector2(tile.X * tileSize, tile.Y * tileSize);

        var center = origin + new Vector2(tileSize / 2f, tileSize / 2f);

        var half = new Vector2(range / 2f, range / 2f);
        return new Box2(center - half, center + half);
    }

    private bool TryGetContext(EntityUid uid, out ZLevelContext ctx)
    {
        ctx = default;

        if (!TryComp(uid, out TransformComponent? xform))
            return false;

        if (xform.MapUid is not { } mapUid)
            return false;

        if (!TryComp(mapUid, out CEZLevelMapComponent? zMap))
            return false;

        if (!_zLevels.TryGetZNetwork(mapUid, out var net) || net == null)
            return false;

        ctx = new ZLevelContext(xform, mapUid, zMap, net.Value.Owner);
        return true;
    }

    private EntityUid? GetNeighborMap(ZLevelContext ctx, int offset)
    {
        var mapEntity = new Entity<CEZLevelMapComponent?>(ctx.MapUid, ctx.ZMap);

        if (!_zLevels.TryMapOffset(mapEntity, offset, out var target) || target == null)
            return null;

        return target.Value.Owner;
    }

    private readonly struct ZLevelContext
    {
        public readonly TransformComponent Transform;
        public readonly EntityUid MapUid;
        public readonly CEZLevelMapComponent ZMap;
        public readonly EntityUid ZNetwork;

        public ZLevelContext(
            TransformComponent transform,
            EntityUid mapUid,
            CEZLevelMapComponent zMap,
            EntityUid zNetwork)
        {
            Transform = transform;
            MapUid = mapUid;
            ZMap = zMap;
            ZNetwork = zNetwork;
        }
    }
    #endregion
}