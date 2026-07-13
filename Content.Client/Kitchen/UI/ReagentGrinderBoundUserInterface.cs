using Content.Shared.Containers.ItemSlots;
using Content.Shared.Kitchen;
using Content.Shared.Kitchen.Components;
using Robust.Client.UserInterface;

namespace Content.Client.Kitchen.UI;

public sealed class ReagentGrinderBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private GrinderMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<GrinderMenu>();
        _menu.SetEntity(Owner);
        _menu.OnToggleAuto += ToggleAutoMode;
        _menu.OnGrind += StartGrinding;
        _menu.OnJuice += StartJuicing;
        _menu.OnEjectAll += EjectAll;
        _menu.OnEjectBeaker += EjectBeaker;
        _menu.OnEjectChamber += EjectChamberContent;
    }

    public override void Update()
    {
        base.Update();

        _menu?.UpdateUi();
    }

<<<<<<< HEAD
=======
    /// <summary>
    ///     Send a message to toggle automatic grinding mode for this grinder.
    /// </summary>
>>>>>>> wizzden/master
    public void ToggleAutoMode()
    {
        SendPredictedMessage(new ReagentGrinderToggleAutoModeMessage());
    }

<<<<<<< HEAD
=======
    /// <summary>
    ///     Send a message to begin grinding the grinder's contents.
    /// </summary>
>>>>>>> wizzden/master
    public void StartGrinding()
    {
        SendPredictedMessage(new ReagentGrinderStartMessage(GrinderProgram.Grind));
    }

<<<<<<< HEAD
=======
    /// <summary>
    ///     Send a message to begin juicing the grinder's contents.
    /// </summary>
>>>>>>> wizzden/master
    public void StartJuicing()
    {
        SendPredictedMessage(new ReagentGrinderStartMessage(GrinderProgram.Juice));
    }

<<<<<<< HEAD
=======
    /// <summary>
    ///     Send a message to request all entities be ejected from the grinder.
    /// </summary>
>>>>>>> wizzden/master
    public void EjectAll()
    {
        SendPredictedMessage(new ReagentGrinderEjectChamberAllMessage());
    }

<<<<<<< HEAD
=======
    /// <summary>
    ///     Send a message to remove the reagent container from the grinder.
    /// </summary>
>>>>>>> wizzden/master
    public void EjectBeaker()
    {
        SendPredictedMessage(new ItemSlotButtonPressedEvent(ReagentGrinderComponent.BeakerSlotId));
    }

<<<<<<< HEAD
=======
    /// <summary>
    ///     Send a message to request a specific entity be ejected from the grinder.
    /// </summary>
    /// <param name="uid">The entity to eject.</param>
>>>>>>> wizzden/master
    public void EjectChamberContent(EntityUid uid)
    {
        SendPredictedMessage(new ReagentGrinderEjectChamberContentMessage(EntMan.GetNetEntity(uid)));
    }
}
