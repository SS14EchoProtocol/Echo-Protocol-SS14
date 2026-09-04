using Content.Shared._Echo.ZLevels;
using Robust.Client.UserInterface;

namespace Content.Client._ECHO.ZLevels.UI;

public sealed partial class ElevatorControllerBoundUserInterface : BoundUserInterface
{
    private ElevatorControllerMenu? _menu;

    public ElevatorControllerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<ElevatorControllerMenu>();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not ElevatorControllerUiState cast)
            return;

        _menu?.Populate(cast.Data, cast.InProgress);
    }
}
