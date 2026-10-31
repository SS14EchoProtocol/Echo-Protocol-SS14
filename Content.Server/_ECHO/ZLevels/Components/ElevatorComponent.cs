namespace Content.Server._Echo.ZLevels;

[RegisterComponent]
public sealed partial class ElevatorComponent : Component
{
    [DataField]
    public string Group = "";

    public bool InProgress = false;
}
