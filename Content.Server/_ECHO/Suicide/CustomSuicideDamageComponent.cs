using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._ECHO.Suicide;

[RegisterComponent]
public sealed partial class CustomSuicideDamageComponent : Component
{
    [DataField(required: true)]
    public ProtoId<DamageTypePrototype> DamageType;
}
