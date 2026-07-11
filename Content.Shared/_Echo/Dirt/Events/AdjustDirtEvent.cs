using Content.Shared.Chemistry.Reagent;
using Content.Shared.Inventory;

namespace Content.Shared._Echo.Dirt;

[ByRefEvent]
public record struct AdjustDirtEvent(ReagentQuantity ReagentQuantity, ReagentPrototype Reagent) : IInventoryRelayEvent
{
    public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;
}
