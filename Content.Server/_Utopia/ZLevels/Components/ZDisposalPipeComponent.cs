using Content.Server._Utopia.ZLevels.Nodes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server._Utopia.ZLevels.Disposal.Components;

[RegisterComponent]
public sealed partial class ZDisposalPipeComponent : Component
{
    [DataField(required: true)]
    public ZNodeDirection ZDirection;
}