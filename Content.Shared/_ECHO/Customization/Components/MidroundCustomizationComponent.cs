using Content.Shared.Body;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._ECHO.Customization;

[RegisterComponent, NetworkedComponent]
public sealed partial class MidroundCustomizationComponent : Component
{
    [DataField(required: true)]
    public HashSet<HumanoidVisualLayers> AllowedLayers = new();

    [DataField(required: true)]
    public HashSet<ProtoId<OrganCategoryPrototype>> Organs;

    [DataField]
    public List<MidroundCustomizationRadialOption> RadialOptions = new();

    [DataField]
    public float AppearanceChangeDuration = 1f;

    [DataField]
    public bool AllowVoiceChange = false;

    public bool AllowAppearanceChange => AllowedLayers.Count > 0;

    [DataField(required: true)]
    public string ActionId = "";

    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid? MenuAction;

    [ViewVariables(VVAccess.ReadOnly)]
    public DoAfterId? AppearanceChangeDoAfter;
}
