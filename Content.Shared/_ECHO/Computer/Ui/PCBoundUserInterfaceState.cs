using Content.Shared.CartridgeLoader;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.Computer;

[Serializable, NetSerializable]
public sealed partial class PCBoundUserInterfaceState : CartridgeLoaderUiState
{
    public ComputerLoginData? Login;

    public PCBoundUserInterfaceState(ComputerLoginData? login, NetEntity? activeUi, List<NetEntity> programs) : base(programs, activeUi)
    {
        Login = login;
    }
}

[NetSerializable, Serializable]
public sealed class ComputerLoginData
{
    public string Username;
    public ProtoId<ComputerAccessPrototype> AccessLevel;

    public ComputerLoginData(string username, ProtoId<ComputerAccessPrototype> accessLevel)
    {
        Username = username;
        AccessLevel = accessLevel;
    }
}


[Serializable, NetSerializable]
public enum PCBoundUiKey : byte
{
    Key
}
