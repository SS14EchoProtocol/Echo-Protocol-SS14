using Content.Shared.CartridgeLoader;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Computer;

[Serializable, NetSerializable]
public sealed class PCTurnOffMessage : BoundUserInterfaceMessage
{
    public PCTurnOffMessage()
    {
    }
}
