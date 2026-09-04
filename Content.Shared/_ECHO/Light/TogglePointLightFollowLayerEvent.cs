using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Light;

[Serializable, NetSerializable, DataDefinition]
public sealed partial class TogglePointLightFollowLayerEvent : EntityEventArgs
{
}
