using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.Atmos;
using Content.Server.Atmos.Components;
using Content.Server.NodeContainer.Nodes;
using Content.Server._Utopia.ZLevels.Nodes;
using Content.Shared._Utopia.ZLevels.Pipes.Components;
using Content.Shared.NodeContainer;
using Content.Shared.Atmos;
using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

namespace Content.Server._Utopia.ZLevels.Pipes.Systems;

public sealed class ZPipeSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;

    private readonly Dictionary<ZPipeNode, HashSet<ZPipeNode>> _connections = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<ZPipeComponent, AtmosDeviceUpdateEvent>(OnAtmosUpdate);
        SubscribeLocalEvent<ZPipeComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnShutdown(EntityUid uid, ZPipeComponent comp, ComponentShutdown args)
    {
        if (TryComp(uid, out NodeContainerComponent? cont))
            ClearAll(cont);
    }

    private void OnAtmosUpdate(EntityUid uid, ZPipeComponent comp, ref AtmosDeviceUpdateEvent args)
    {
        if (!TryComp(uid, out NodeContainerComponent? cont))
            return;

        foreach (var node in GetZNodes(cont))
        {
            if (!_connections.TryGetValue(node, out var set))
                continue;

            foreach (var other in set)
            {
                if (node.Owner.Id < other.Owner.Id)
                    TransferGas(node, other);
            }
        }
    }

    public void AddZConnection(ZPipeNode a, ZPipeNode b)
    {
        GetOrAdd(a).Add(b);
        GetOrAdd(b).Add(a);
    }

    public void ClearAll(NodeContainerComponent cont)
    {
        foreach (var z in GetZNodes(cont))
            ClearConnections(z);
    }

    private void TransferGas(ZPipeNode a, ZPipeNode b)
    {
        var airA = a.Air;
        var airB = b.Air;

        var deltaP = airA.Pressure - airB.Pressure;
        if (MathF.Abs(deltaP) < 0.01f)
            return;

        var src = deltaP > 0 ? airA : airB;
        var dst = deltaP > 0 ? airB : airA;

        var T = src.Temperature;
        var V = src.Volume;
        if (T <= 0f || V <= 0f)
            return;

        var dn = MathF.Min(
            (MathF.Abs(deltaP) * V) / (Atmospherics.R * T),
            src.TotalMoles);

        if (dn <= 0f)
            return;

        var removed = src.Remove(dn);
        _atmosphere.Merge(dst, removed);
    }

    private HashSet<ZPipeNode> GetOrAdd(ZPipeNode node)
    {
        if (!_connections.TryGetValue(node, out var set))
        {
            set = new HashSet<ZPipeNode>();
            _connections[node] = set;
        }

        return set;
    }

    private void ClearConnections(ZPipeNode node)
    {
        if (!_connections.Remove(node, out var set))
            return;

        foreach (var other in set)
        {
            if (_connections.TryGetValue(other, out var otherSet))
            {
                otherSet.Remove(node);
                if (otherSet.Count == 0)
                    _connections.Remove(other);
            }
        }
    }

    private static IEnumerable<ZPipeNode> GetZNodes(NodeContainerComponent cont)
    {
        foreach (var node in cont.Nodes.Values)
            if (node is ZPipeNode z)
                yield return z;
    }
}
