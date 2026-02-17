using Robust.Shared.Containers;

namespace Content.Shared._Echo.ContainerSprite;

public sealed class ContainerSpriteSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ContainerSpriteComponent, EntGotInsertedIntoContainerMessage>(OnContainerInserted);
        SubscribeLocalEvent<ContainerSpriteComponent, EntGotRemovedFromContainerMessage>(OnContainerRemoved);
    }

    private void OnContainerInserted(Entity<ContainerSpriteComponent> ent, ref EntGotInsertedIntoContainerMessage args)
    {
        _appearance.SetData(ent.Owner, ContainerSpriteState.State, true);
    }
    private void OnContainerRemoved(Entity<ContainerSpriteComponent> ent, ref EntGotRemovedFromContainerMessage args)
    {
        _appearance.SetData(ent.Owner, ContainerSpriteState.State, false);
    }
}
