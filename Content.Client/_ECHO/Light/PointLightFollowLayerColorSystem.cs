using System.Collections.Generic;
using Content.Shared._ECHO.Light;
using Content.Shared.Body;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;

namespace Content.Client._ECHO.Light;

public sealed partial class PointLightFollowLayerColorSystem : SharedPointLightFollowLayerColorSystem
{
    [Dependency] private SpriteSystem _sprite = default!;
    [Dependency] private PointLightSystem _pointLight = default!;
    [Dependency] private SharedContainerSystem _container = default!;

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        var query = EntityQueryEnumerator<PointLightComponent, PointLightFollowLayerColorComponent>();
        while (query.MoveNext(out var uid, out _, out var follow))
        {
            if (!follow.Enabled)
            {
                _pointLight.SetColor(uid, Color.White);
                continue;
            }

            if (TryGetColor(uid, follow, out var color))
            {
                _pointLight.SetColor(uid, color);
            }
        }
    }

    private bool TryGetColor(EntityUid uid, PointLightFollowLayerColorComponent follow, out Color color)
    {
        color = Color.White;

        if (TryGetColorFromVisualBody(uid, follow, out color))
            return true;

        if (follow.StringLayer != null && _sprite.TryGetLayer(uid, follow.StringLayer, out var layerFromString, false))
        {
            color = layerFromString.Color;
            return true;
        }

        if (follow.EnumLayer != null && _sprite.TryGetLayer(uid, follow.EnumLayer, out var layerFromEnum, false))
        {
            color = layerFromEnum.Color;
            return true;
        }

        if (follow.IdLayer != null && _sprite.TryGetLayer(uid, follow.IdLayer.Value, out var layerFromId, false))
        {
            color = layerFromId.Color;
            return true;
        }

        return false;
    }

    private bool TryGetColorFromVisualBody(EntityUid uid, PointLightFollowLayerColorComponent follow, out Color color)
    {
        color = Color.White;

        if (follow.EnumLayer is not HumanoidVisualLayers layerToFollow)
            return false;

        if (!TryComp<VisualBodyComponent>(uid, out _))
            return false;

        if (!_container.TryGetContainer(uid, BodyComponent.ContainerID, out var container) || container is not Container actualContainer)
            return false;

        foreach (var organ in actualContainer.ContainedEntities)
        {
            if (!TryComp<VisualOrganMarkingsComponent>(organ, out var markingsComp))
                continue;

            if (TryGetColorFromMarkings(markingsComp.Markings, follow.EnumLayer, out color))
                return true;
        }

        return false;
    }

    internal static bool TryGetColorFromMarkings(IReadOnlyDictionary<HumanoidVisualLayers, List<Marking>> markings, Enum? targetLayer, out Color color)
    {
        color = Color.White;

        if (targetLayer is not HumanoidVisualLayers layerToFollow)
            return false;

        if (!markings.TryGetValue(layerToFollow, out var layerMarkings))
            return false;

        return TryGetColorFromMarkings(layerMarkings, out color);
    }

    internal static bool TryGetColorFromMarkings(IReadOnlyList<Marking> layerMarkings, out Color color)
    {
        color = Color.White;

        foreach (var marking in layerMarkings)
        {
            if (marking.MarkingColors.Count == 0)
                continue;

            color = marking.MarkingColors[0];
            return true;
        }

        return false;
    }
}
