using Content.Shared.Body;
using Content.Shared.ECHO.SpeechBarks;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Customization;

[Serializable, NetSerializable]
public sealed partial class MidroundCustomizationBoundUserInterfaceState : BoundUserInterfaceState
{
    public readonly Dictionary<ProtoId<OrganCategoryPrototype>, OrganProfileData> OrganProfileData;
    public readonly Dictionary<ProtoId<OrganCategoryPrototype>, OrganMarkingData> OrganMarkingData;
    public readonly Dictionary<ProtoId<OrganCategoryPrototype>, Dictionary<HumanoidVisualLayers, List<Marking>>> AppliedMarkings;
    public readonly HashSet<HumanoidVisualLayers> AllowedLayers;
    public readonly BarkData? SelectedBark;

    public MidroundCustomizationBoundUserInterfaceState(Dictionary<ProtoId<OrganCategoryPrototype>, OrganProfileData> profiles,
                                                         Dictionary<ProtoId<OrganCategoryPrototype>, OrganMarkingData> markings,
                                                         Dictionary<ProtoId<OrganCategoryPrototype>, Dictionary<HumanoidVisualLayers, List<Marking>>> applied,
                                                         HashSet<HumanoidVisualLayers> allowedLayers,
                                                         BarkData? selectedBark)
    {
        OrganProfileData = profiles;
        OrganMarkingData = markings;
        AppliedMarkings = applied;
        AllowedLayers = allowedLayers;
        SelectedBark = selectedBark;
    }
}

[Serializable, NetSerializable]
public enum MidroundCustomizationAppearanceUiKey
{
    Key
}

[Serializable, NetSerializable]
public enum MidroundCustomizatioBarksUiKey
{
    Key
}
