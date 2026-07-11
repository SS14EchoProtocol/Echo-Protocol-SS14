using System.Diagnostics.CodeAnalysis;
using Content.Shared._Echo.Dirt;
using Content.Shared.Clothing;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client._Echo.Dirt;

public sealed class DirtSystem : SharedDirtSystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DirtVisualsComponent, AfterAutoHandleStateEvent>(OnHandleState);
        SubscribeLocalEvent<DirtVisualsComponent, GetEquipmentVisualsEvent>(OnGetEquipmentVisuals);
    }

    private void OnHandleState(Entity<DirtVisualsComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (!_sprite.TryGetLayer(ent.Owner, DirtVisualsComponent.DirtLayer, out var layer, false))
            return;

        if (!TryGetDirtLayer(ent, out var dirtLayer, out var color))
        {
            _sprite.LayerSetVisible(ent.Owner, DirtVisualsComponent.DirtLayer, false);
            return;
        }

        dirtLayer.Color = color;

        _sprite.LayerSetData(ent.Owner, DirtVisualsComponent.DirtLayer, dirtLayer);
        _sprite.LayerSetVisible(ent.Owner, DirtVisualsComponent.DirtLayer, true);
    }

    private void OnGetEquipmentVisuals(Entity<DirtVisualsComponent> ent, ref GetEquipmentVisualsEvent args)
    {
        if (!TryGetDirtLayer(ent, out var layer, out var color))
            return;

        layer.Color = color;
        args.Layers.Add((DirtVisualsComponent.DirtLayer, layer));
    }

    private bool TryGetDirtLayer(Entity<DirtVisualsComponent> ent, [NotNullWhen(true)] out PrototypeLayerData? layer, [NotNullWhen(true)] out Color? color)
    {
        layer = null;
        color = null;

        if (!Solution.TryGetSolution(ent.Owner, DirtVisualsComponent.DirtSolution, out var solution))
            return false;

        var dirtAmount = DirtAmount.None;
        foreach (var (threshold, amount) in ent.Comp.DirtThresholds)
        {
            if (solution.Value.Comp.Solution.Volume < amount)
                continue;

            dirtAmount = threshold;
        }

        if (dirtAmount == DirtAmount.None)
            return false;

        layer = ent.Comp.DirtLayers[dirtAmount];
        color = solution.Value.Comp.Solution.GetColor(_proto);
        return true;
    }
}
