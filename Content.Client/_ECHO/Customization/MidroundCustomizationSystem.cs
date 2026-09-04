using Content.Client._ECHO.Customization.UI;
using Content.Shared._ECHO.Customization;
using Robust.Client.UserInterface;
using Robust.Shared.Timing;

namespace Content.Client._ECHO.Customization;

public sealed partial class MidroundCustomizationSystem : SharedMidroundCustomizationSystem
{
    [Dependency] private IUserInterfaceManager _ui = default!;
    [Dependency] private IGameTiming _timing = default!;

    protected override void OpenRadialMenu(List<MidroundCustomizationRadialOption> options)
    {
        base.OpenRadialMenu(options);

        if (!_timing.IsFirstTimePredicted)
            return;

        _ui.GetUIController<MidroundCustomizationRadialMenuController>().TryToggleMenu(options);
    }
}
