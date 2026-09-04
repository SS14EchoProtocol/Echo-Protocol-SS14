using Content.Client.UserInterface.Controls;
using Content.Shared._ECHO.Customization;
using JetBrains.Annotations;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client._ECHO.Customization.UI;

[UsedImplicitly]
public sealed partial class MidroundCustomizationRadialMenuController : UIController
{
    [Dependency] private IPlayerManager _playerManager = default!;

    private SimpleRadialMenu? _menu;

    public void TryToggleMenu(List<MidroundCustomizationRadialOption> options)
    {
        if (_menu == null)
        {
            var models = new List<RadialMenuActionOption<MidroundCustomizationRadialOption>>();
            foreach (var option in options)
            {
                models.Add(new(OnPressed, option)
                {
                    ToolTip = Loc.GetString(option.OptionName),
                    IconSpecifier = RadialMenuIconSpecifier.With(option.Icon)
                });
            }

            _menu = new SimpleRadialMenu();

            _menu.SetButtons(models);

            _menu.OnClose += OnClose;

            _menu.Open();

            _menu.OpenOverMouseScreenPosition();
        }
        else
        {
            CloseMenu();
        }
    }

    private void OnPressed(MidroundCustomizationRadialOption option)
    {
        if (_playerManager.LocalEntity is not { Valid: true } player)
            return;

        var netPlayer = EntityManager.GetNetEntity(player);
        var ev = new MidroundCustomizationOptionSelectedEvent(netPlayer, option);
        EntityManager.RaisePredictiveEvent(ev);
    }

    private void CloseMenu()
    {
        if (_menu == null)
            return;

        _menu.Close();
        _menu = null;
    }

    private void OnClose()
    {
        _menu?.OnClose -= OnClose;
        _menu = null;
    }
}
