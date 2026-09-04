using System.Linq;
using Content.Shared.Movement.Systems;
using Content.Shared.Power;
using Content.Shared.Power.EntitySystems;
using Content.Shared.PowerCell;
using Robust.Shared.Containers;

namespace Content.Shared._ECHO.Battery;

public sealed partial class ChargeMoveSpeedThresholdsSystems : EntitySystem
{
    [Dependency] private MovementSpeedModifierSystem _moveSpeedModifiers = default!;
    [Dependency] private PowerCellSystem _powerCell = default!;
    [Dependency] private SharedBatterySystem _battery = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChargeMoveSpeedThresholdsComponent, ChargeChangedEvent>(RefreshModifiers);
        SubscribeLocalEvent<ChargeMoveSpeedThresholdsComponent, EntRemovedFromContainerMessage>(RefreshModifiers);
        SubscribeLocalEvent<ChargeMoveSpeedThresholdsComponent, EntInsertedIntoContainerMessage>(RefreshModifiers);
        SubscribeLocalEvent<ChargeMoveSpeedThresholdsComponent, AfterAutoHandleStateEvent>(RefreshModifiers);

        SubscribeLocalEvent<ChargeMoveSpeedThresholdsComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshModifiers);
    }

    private void RefreshModifiers<T>(Entity<ChargeMoveSpeedThresholdsComponent> ent, ref T args)
    {
        _moveSpeedModifiers.RefreshMovementSpeedModifiers(ent.Owner);
    }

    private void OnRefreshModifiers(Entity<ChargeMoveSpeedThresholdsComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        if (!_powerCell.TryGetBatteryFromSlotOrEntity(ent.Owner, out var battery))
        {
            var minThreshold = ent.Comp.SpeedModifierThresholds.Min(x => x.Key);
            args.ModifySpeed(ent.Comp.SpeedModifierThresholds[minThreshold]);
            return;
        }

        var chargeLevel = _battery.GetChargeLevel(battery.Value.Owner);

        float? closestThreshold = null;
        float resultMod = 1f;
        foreach (var item in ent.Comp.SpeedModifierThresholds)
        {
            if (item.Key > chargeLevel)
                continue;

            if (closestThreshold == null || item.Key > closestThreshold)
            {
                closestThreshold = item.Key;
                resultMod = item.Value;
            }
        }

        if (closestThreshold == null && ent.Comp.SpeedModifierThresholds.Count > 0)
        {
            var minThreshold = ent.Comp.SpeedModifierThresholds.Min(x => x.Key);
            resultMod = ent.Comp.SpeedModifierThresholds[minThreshold];
        }

        args.ModifySpeed(resultMod);
    }
}
