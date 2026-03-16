using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared._Utopia.ZLevels.Transmission.Components;

[RegisterComponent]
public sealed partial class ZLevelEntityLinkComponent : Component
{
    public EntityUid? ZNetwork;
    public EntityUid? MapEntity;

    public EntityUid? GridEntity;
    public EntityUid? AboveMap;
    public EntityUid? BelowMap;

    public int Depth;
}