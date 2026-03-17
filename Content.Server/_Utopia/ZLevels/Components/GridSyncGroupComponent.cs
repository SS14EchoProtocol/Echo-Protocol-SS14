using Robust.Shared.GameObjects;
using System.Collections.Generic;

namespace Content.Server._Utopia.ZLevels.Components;

[RegisterComponent]
public sealed partial class GridSyncGroupComponent : Component
{
    public List<EntityUid> Proxies = new();
}
