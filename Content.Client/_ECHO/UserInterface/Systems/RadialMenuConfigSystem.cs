using Content.Client.UserInterface.Controls;
using Content.Shared._ECHO.UserInterface;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;

namespace Content.Client._ECHO.UserInterface;

public sealed partial class RadialMenuConfigSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = default!;

    public override void Initialize()
    {
        base.Initialize();

        _cfg.OnValueChanged(EchoCCVars.RadialMenuConfig, OnConfigChanged, true);
    }

    private void OnConfigChanged(int value)
    {
        RadialMenu.RadialMenuConfig = (RadialMenuType)value;
    }
}
