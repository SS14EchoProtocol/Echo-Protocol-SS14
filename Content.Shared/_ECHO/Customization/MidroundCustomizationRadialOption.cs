using Robust.Shared.Graphics;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._ECHO.Customization;

[Serializable, NetSerializable, DataDefinition]
public sealed partial class MidroundCustomizationRadialOption
{
    [DataField(required: true)]
    public SpriteSpecifier Icon = default!;

    [DataField(required: true)]
    public string OptionName = default!;

    [DataField]
    public Enum? UiKey = null;

    [DataField]
    public object? Event;
}
