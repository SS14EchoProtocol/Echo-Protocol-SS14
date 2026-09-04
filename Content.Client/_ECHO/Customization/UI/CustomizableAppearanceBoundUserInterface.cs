using Content.Client.Humanoid;
using Content.Shared._ECHO.Customization;
using Content.Shared.MagicMirror;
using Robust.Client.UserInterface;

namespace Content.Client._ECHO.Customization.UI;

public sealed class CustomizableAppearanceBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private CustomizableAppearanceWindow? _window;

    private readonly MarkingsViewModel _markingsModel = new();

    public CustomizableAppearanceBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<CustomizableAppearanceWindow>();
        _window.MarkingsPicker.SetModel(_markingsModel);

        _window.OnApplyPressed += () =>
        {
            SendPredictedMessage(new MidroundCustomizationSelectMarkingMessage(_markingsModel.Markings));
            Close();
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not MidroundCustomizationBoundUserInterfaceState data)
            return;

        _markingsModel.OrganData = data.OrganMarkingData;
        _markingsModel.OrganProfileData = data.OrganProfileData;
        _markingsModel.Markings = data.AppliedMarkings;
        _markingsModel.LayersWhitelist = data.AllowedLayers;
    }
}
