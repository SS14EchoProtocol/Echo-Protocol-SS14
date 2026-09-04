using Content.Server.Power.EntitySystems;
using Content.Shared._ECHO.Radio;
using Content.Shared.Chat;
using Content.Shared.Power;
using Content.Shared.PowerCell;
using Content.Shared.Radio.Components;
using Robust.Shared.Player;

namespace Content.Server.Radio.EntitySystems;

public sealed partial class RadioSystem
{
    [Dependency] private PowerCellSystem _powerCell = default!;
    [Dependency] private BatterySystem _battery = default!;

    private void InitializeEcho()
    {
        SubscribeLocalEvent<BatteryIntrinsicRadioTransmitterComponent, ChargeChangedEvent>(OnBatteryIntrinsicChargeChanged);
        SubscribeLocalEvent<BatteryIntrinsicRadioReceiverComponent, RadioReceiveEvent>(OnBatteryIntrinsicReceive);
    }

    private void OnBatteryIntrinsicChargeChanged(Entity<BatteryIntrinsicRadioTransmitterComponent> ent, ref ChargeChangedEvent args)
    {
        var chargeLevel = args.CurrentCharge / args.MaxCharge;
        var transmitter = EnsureComp<IntrinsicRadioTransmitterComponent>(ent.Owner);

        if (args.CurrentCharge == 0)
        {
            transmitter.CanTransmit = false;
            return;
        }

        transmitter.CanTransmit = chargeLevel > ent.Comp.ChargeThreshold;
    }

    private void OnBatteryIntrinsicReceive(Entity<BatteryIntrinsicRadioReceiverComponent> ent, ref RadioReceiveEvent args)
    {
        if (!TryComp<ActorComponent>(ent.Owner, out var actor))
            return;

        if (!_powerCell.TryGetBatteryFromSlotOrEntity(ent.Owner, out var battery) || _battery.GetChargeLevel(battery.Value.Owner) <= ent.Comp.ChargeThreshold)
            return;

        _netMan.ServerSendMessage(args.ChatMsg, actor.PlayerSession.Channel);
    }

}
