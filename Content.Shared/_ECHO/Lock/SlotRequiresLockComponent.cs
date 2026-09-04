using Robust.Shared.GameStates;

namespace Content.Shared._ECHO.Lock;

[RegisterComponent, NetworkedComponent]
public sealed partial class SlotRequiresLockComponent : Component
{
    [DataField(required: true)]
    public string SlotId = default!;
}
