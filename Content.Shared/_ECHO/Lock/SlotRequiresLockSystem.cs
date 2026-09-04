using Content.Shared.Containers.ItemSlots;
using Content.Shared.Lock;

namespace Content.Shared._ECHO.Lock;

public sealed partial class SlotRequiresLockSystem : EntitySystem
{
    [Dependency] private ItemSlotsSystem _slots = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SlotRequiresLockComponent, LockToggledEvent>(OnLockToggle);
    }

    private void OnLockToggle(Entity<SlotRequiresLockComponent> ent, ref LockToggledEvent args)
    {
        _slots.SetLock(ent.Owner, ent.Comp.SlotId, args.Locked);
    }
}
