using Content.Client.CartridgeLoader;
using Content.Shared._ECHO.Computer;
using Content.Shared.CartridgeLoader;
using Robust.Client.UserInterface;

namespace Content.Client._ECHO.Computer.UI;

public sealed partial class ComputerBoundUserInterface : CartridgeLoaderBoundUserInterface
{
    private ComputerWindow? _menu;

    public ComputerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        if (_menu != null)
            return;

        _menu = this.CreateWindow<ComputerWindow>();

        _menu.OnProgramItemPressed += ActivateCartridge;
        _menu.OnInstallButtonPressed += InstallCartridge;
        _menu.OnUninstallButtonPressed += UninstallCartridge;
        _menu.OnCloseItemPressed += DeactivateActiveCartridge;
        _menu.OnLoginButtonPressed += SendLogin;
        _menu.OnLogOutButtonPressed += LogOut;
        _menu.TurnOffComputer += () => SendMessage(new PCTurnOffMessage());

        _menu.OpenCentered();

        base.Open();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not PCBoundUserInterfaceState computerState)
            return;

        if (computerState.Login != null)
            _menu?.Login(computerState.Login);
        else
            _menu?.Logout();
    }

    protected override void AttachCartridgeUI(Control cartridgeUIFragment, string? title)
    {
        if (_menu is null)
            return;

        _menu.SetOpenedWindow(cartridgeUIFragment, title);
    }

    protected override void DetachCartridgeUI(Control cartridgeUIFragment)
    {
        if (_menu is null)
            return;

        _menu.SetOpenedWindow(null, null);
    }

    protected override void UpdateAvailablePrograms(List<(EntityUid, CartridgeComponent)> programs)
    {
        _menu?.UpdateAvailablePrograms(programs);
    }

    private void SendLogin(string username, string password)
    {
        var message = new PCLoginUiMessage(username, password);
        SendMessage(message);
    }

    private void LogOut()
    {
        var message = new PCLogOutMessage();
        SendMessage(message);
    }
}
