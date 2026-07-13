using Content.Client.Atmos.Overlays;
using Content.Shared.Inventory.Events;
using Content.Shared.Overlays;
using Robust.Client.Graphics;

namespace Content.Client.Overlays;

public sealed partial class ThermalSightOverlaySystem : EquipmentHudSystem<ThermalSightComponent>
{
<<<<<<< HEAD
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
=======
    [Dependency] private IOverlayManager _overlayMan = default!;
>>>>>>> wizzden/master

    private GasTileDangerousTemperatureOverlay _temperatureOverlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        _temperatureOverlay = new();
    }

    protected override void UpdateInternal(RefreshEquipmentHudEvent<ThermalSightComponent> component)
    {
        base.UpdateInternal(component);

        _overlayMan.AddOverlay(_temperatureOverlay);
    }

    protected override void DeactivateInternal()
    {
        base.DeactivateInternal();

        _overlayMan.RemoveOverlay(_temperatureOverlay);
    }
}
