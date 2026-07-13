
using Content.Shared.Preferences;

namespace Content.Client.Lobby.UI;

public sealed partial class HumanoidProfileEditor
{
    private void SetName(string newName)
    {
        Profile = Profile?.WithName(newName);
        SetDirty();

        if (!IsDirty)
            return;

        SpriteView.SetName(newName);
    }

    private void UpdateNameEdit()
    {
        NameEdit.Text = Profile?.Name ?? "";
    }

<<<<<<< HEAD
    private void RandomizeEverything()
    {
        Profile = HumanoidCharacterProfile.Random();
        SetProfile(Profile, CharacterSlot);
        SetDirty();
    }

    private void RandomizeName()
    {
        if (Profile == null) return;
        var name = HumanoidCharacterProfile.GetName(Profile.Species, Profile.Gender);
        SetName(name);
        UpdateNameEdit();
    }
=======
    /// <summary>
    /// Randomize values selectively while respecting locked values.
    /// </summary>
    private void RandomizeProfile()
    {
        Profile = Profile == null
            ? HumanoidCharacterProfile.Random()
            : HumanoidCharacterProfile.Random(RandomizeLockButton.RandomizeCfg, Profile!);
        SetProfile(Profile, CharacterSlot);
        SetDirty();
    }
>>>>>>> wizzden/master
}
