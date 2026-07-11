using Content.Shared.Chemistry;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Inventory;
using Content.Shared.Item;

namespace Content.Shared._Echo.Dirt;

public abstract class SharedDirtSystem : EntitySystem
{
    [Dependency] protected readonly SharedSolutionContainerSystem Solution = default!;
    [Dependency] private readonly SharedItemSystem _item = default!;

    private const float DirtPerUnit = 0.6f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DirtVisualsComponent, ReactionEntityEvent>(OnReactionEntity);
        SubscribeLocalEvent<DirtVisualsComponent, AdjustDirtEvent>(OnAdjustDirt);
        SubscribeLocalEvent<DirtVisualsComponent, InventoryRelayedEvent<AdjustDirtEvent>>(OnAdjustDirt);
    }

    private void OnReactionEntity(Entity<DirtVisualsComponent> ent, ref ReactionEntityEvent args)
    {
        if (args.Method != ReactionMethod.Touch)
            return;

        var ev = new AdjustDirtEvent(args.ReagentQuantity, args.Reagent);
        RaiseLocalEvent(ent.Owner, ref ev);
    }

    private void OnAdjustDirt(Entity<DirtVisualsComponent> ent, ref AdjustDirtEvent args)
    {
        if (args.ReagentQuantity.Quantity <= 0)
            return;

        if (!Solution.TryGetSolution(ent.Owner, DirtVisualsComponent.DirtSolution, out var solution))
            return;

        var dirtAmount = args.ReagentQuantity.Quantity * DirtPerUnit;
        Solution.TryAddReagent(solution.Value, new ReagentQuantity(args.ReagentQuantity.Reagent, dirtAmount), out _);
        _item.VisualsChanged(ent.Owner);
    }

    private void OnAdjustDirt(Entity<DirtVisualsComponent> ent, ref InventoryRelayedEvent<AdjustDirtEvent> args)
    {
        if (args.Args.ReagentQuantity.Quantity <= 0)
            return;

        if (!Solution.TryGetSolution(ent.Owner, DirtVisualsComponent.DirtSolution, out var solution))
            return;

        var dirtAmount = args.Args.ReagentQuantity.Quantity * DirtPerUnit;
        Solution.TryAddReagent(solution.Value, new ReagentQuantity(args.Args.ReagentQuantity.Reagent, dirtAmount), out _);
        _item.VisualsChanged(ent.Owner);
    }
}
