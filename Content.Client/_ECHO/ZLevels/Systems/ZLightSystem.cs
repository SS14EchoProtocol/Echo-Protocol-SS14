using System.Linq;
using Robust.Client.GameObjects;

namespace Content.Client._ECHO.ZLevels;

public sealed class ZLightSystem : EntitySystem
{
    [Dependency] private readonly PointLightSystem _lightSys = default!;
    [Dependency] private readonly ContainerSystem _container = default!;

    private Dictionary<Entity<PointLightComponent>, Entity<ZLightMimicComponent>> _mimics = new();

    public override void FrameUpdate(float frameTime)
    {
        for (var i = _mimics.Count - 1; i >= 0; i--)
        {
            if (_mimics.ElementAt(i).Key.Owner is not { Valid: true })
            {
                QueueDel(_mimics.ElementAt(i).Value);
            }
        }

        _mimics.Remove(new Entity<PointLightComponent>());

        var query = EntityQueryEnumerator<PointLightComponent>();
        while (query.MoveNext(out var uid, out var light))
        {

        }
    }

    private void EnsureMimic(Entity<PointLightComponent> ent)
    {
        if (_mimics.TryGetValue(ent, out var mimic))
        {
            _lightSys.SetCastShadows(mimic, ent.Comp.CastShadows);
            _lightSys.SetColor(mimic, ent.Comp.Color);
            _lightSys.SetCurveFactor(mimic, ent.Comp.CurveFactor);
            _lightSys.SetEnergy(mimic, ent.Comp.Energy);
            _lightSys.SetFalloff(mimic, ent.Comp.Falloff);
            _lightSys.SetMask(ent.Comp.MaskPath, Comp<PointLightComponent>(mimic));
            _lightSys.SetRadius(mimic, ent.Comp.Radius);
            _lightSys.SetSoftness(mimic, ent.Comp.Radius);
            _lightSys.SetEnabled(mimic, false);
        }
        else if (!ent.Comp.ContainerOccluded || !_container.IsEntityInContainer(ent.Owner))
        {
            mimic = Spawn("");
        }
        else
        {
            return;
        }
    }

    private void ClearMimics()
    {

    }
}
