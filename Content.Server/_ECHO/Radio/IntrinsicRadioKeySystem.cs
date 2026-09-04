using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.ADT.Radio;

public sealed class IntrinsicRadioKeySystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<IntrinsicRadioTransmitterComponent, EncryptionChannelsChangedEvent>(OnTransmitterChannelsChanged);
        SubscribeLocalEvent<ActiveRadioComponent, EncryptionChannelsChangedEvent>(OnReceiverChannelsChanged);
    }

    private void OnTransmitterChannelsChanged(Entity<IntrinsicRadioTransmitterComponent> ent, ref EncryptionChannelsChangedEvent args)
    {
        UpdateChannels(args.Component, ref ent.Comp.Channels);
    }

    private void OnReceiverChannelsChanged(Entity<ActiveRadioComponent> ent, ref EncryptionChannelsChangedEvent args)
    {
        UpdateChannels(args.Component, ref ent.Comp.Channels);
    }

    private void UpdateChannels(EncryptionKeyHolderComponent comp, ref HashSet<ProtoId<RadioChannelPrototype>> channels)
    {
        channels.Clear();
        channels.UnionWith(comp.Channels);
    }
}
