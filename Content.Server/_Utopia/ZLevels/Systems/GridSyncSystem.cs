using Content.Server._Utopia.ZLevels.Components;
using Content.Server._Utopia.ZLevels.Events;

namespace Content.Server._Utopia.ZLevels.Systems;

public sealed class GridSyncSystem : EntitySystem
{
    public void Broadcast(EntityUid group, EntityUid sender, GridMotionCommandEvent command)
    {
        if (!TryComp(group, out GridSyncGroupComponent? grp))
            return;

        foreach (var proxy in grp.Proxies)
        {
            if (proxy == sender)
                continue;

            RaiseLocalEvent(proxy, command);
        }
    }
}
