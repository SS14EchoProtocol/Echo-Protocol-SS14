using Robust.Shared.Serialization;

namespace Content.Shared._Echo.ContainerSprite;

[RegisterComponent]
public sealed partial class ContainerSpriteComponent : Component
{
}

[Serializable, NetSerializable]
public enum ContainerSpriteState
{
    State
}
