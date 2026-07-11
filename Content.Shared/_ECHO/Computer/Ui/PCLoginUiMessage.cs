using Content.Shared.CartridgeLoader;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Computer;

[Serializable, NetSerializable]
public sealed class PCLoginUiMessage : BoundUserInterfaceMessage
{
    public string Username;
    public string Password;

    public PCLoginUiMessage(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
