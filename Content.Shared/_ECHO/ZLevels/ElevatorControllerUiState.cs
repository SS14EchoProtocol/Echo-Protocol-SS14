using Robust.Shared.Serialization;

namespace Content.Shared._Echo.ZLevels;

[Serializable, NetSerializable]
public sealed partial class ElevatorControllerUiState : BoundUserInterfaceState
{
    public readonly List<ElevatorFloorData> Data;
    public readonly bool InProgress;

    public ElevatorControllerUiState(List<ElevatorFloorData> data, bool inProgress)
    {
        Data = data;
        InProgress = inProgress;
    }
}

[Serializable, NetSerializable]
public enum ElevatorControllerUiKey : byte
{
    Key
}
