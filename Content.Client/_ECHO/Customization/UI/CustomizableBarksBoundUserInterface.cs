using Content.Client._ECHO.Barks;
using Content.Client.Humanoid;
using Content.Client.UserInterface.Controls;
using Content.Shared._ECHO.Customization;
using Content.Shared.ECHO.SpeechBarks;
using Content.Shared.MagicMirror;
using Robust.Client.UserInterface;

namespace Content.Client._ECHO.Customization.UI;

public sealed class CustomizableBarksBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private CustomizableBarksWindow? _window;

    private BarkData? _bark;

    public CustomizableBarksBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<CustomizableBarksWindow>();

        _window.Tab.OnBarkSelected += OnBarkProto;
        _window.Tab.OnPitchChanged += OnBarkPitch;
        _window.Tab.OnMinVarChanged += OnBarkMinVar;
        _window.Tab.OnMaxVarChanged += OnBarkMaxVar;
        _window.OnApplyPressed += OnApply;

        _window.OnClose += OnClose;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not MidroundCustomizationBoundUserInterfaceState data)
            return;

        if (data.SelectedBark == null)
        {
            _window?.Close();
            return;
        }

        _bark = data.SelectedBark;
        _window?.PopulateTab(data.SelectedBark.Proto, data.SelectedBark.Pitch, data.SelectedBark.MinVar, data.SelectedBark.MaxVar);
    }

    private void OnClose()
    {
        _window?.Tab.OnBarkSelected -= OnBarkProto;
        _window?.Tab.OnPitchChanged -= OnBarkPitch;
        _window?.Tab.OnMinVarChanged -= OnBarkMinVar;
        _window?.Tab.OnMaxVarChanged -= OnBarkMaxVar;
        _window?.OnApplyPressed -= OnApply;
    }

    private void OnBarkProto(string protoId)
    {
        if (_bark == null)
            return;

        _bark.Proto = protoId;
    }

    private void OnBarkPitch(float pitch)
    {
        if (_bark == null)
            return;

        _bark.Pitch = pitch;
    }

    private void OnBarkMinVar(float variation)
    {
        if (_bark == null)
            return;

        _bark.MinVar = variation;
    }

    private void OnBarkMaxVar(float variation)
    {
        if (_bark == null)
            return;

        _bark.MaxVar = variation;
    }

    private void OnApply()
    {
        if (_bark == null)
            return;

        SendMessage(new MidroundCustomizationSetBarkMessage(_bark));
        Close();
    }
}
