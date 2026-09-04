using Content.Shared.Actions;
using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Customization;

[Serializable, NetSerializable]
public sealed partial class MidroundCustomizationOptionSelectedEvent : EntityEventArgs
{
    public readonly NetEntity Sender;
    public readonly MidroundCustomizationRadialOption Option;

    public MidroundCustomizationOptionSelectedEvent(NetEntity sender, MidroundCustomizationRadialOption option)
    {
        Sender = sender;
        Option = option;
    }
}
