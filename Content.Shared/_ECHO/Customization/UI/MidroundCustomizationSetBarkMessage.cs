using Content.Shared.Body;
using Content.Shared.ECHO.SpeechBarks;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Customization;

[Serializable, NetSerializable]
public sealed class MidroundCustomizationSetBarkMessage : BoundUserInterfaceMessage
{
    public readonly BarkData NewBark;

    public MidroundCustomizationSetBarkMessage(BarkData bark)
    {
        NewBark = bark;
    }
}
