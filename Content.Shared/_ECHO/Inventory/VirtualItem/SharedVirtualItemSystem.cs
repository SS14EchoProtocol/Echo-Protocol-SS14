using Content.Shared.Hands;

namespace Content.Shared.Inventory.VirtualItem;

public abstract partial class SharedVirtualItemSystem
{
    public bool TryDeleteVirtualItem(Entity<VirtualItemComponent> item, EntityUid user)
    {
        var userEv = new BeforeVirtualItemDeletedEvent(item.Comp.BlockingEntity, user);
        RaiseLocalEvent(user, userEv);

        var targEv = new BeforeVirtualItemDeletedEvent(item.Comp.BlockingEntity, user);
        RaiseLocalEvent(item.Comp.BlockingEntity, targEv);

        if (userEv.Cancelled || targEv.Cancelled)
            return false;

        DeleteVirtualItem(item, user);
        return true;
    }
}
