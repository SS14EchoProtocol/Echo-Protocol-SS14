namespace Content.Server.Silicons.Laws;

<<<<<<< HEAD
public sealed class IonLawLocalizationSystem : EntitySystem
{
    [Dependency] private readonly ILocalizationManager _loc = default!;
    [Dependency] private readonly IonLawSystem _ionLaw = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;
=======
public sealed partial class IonLawLocalizationSystem : EntitySystem
{
    [Dependency] private IonLawSystem _ionLaw = default!;
>>>>>>> wizzden/master

    public override void Initialize()
    {
        base.Initialize();

<<<<<<< HEAD
        _sawmill = _logManager.GetSawmill("ion-law");

        var culture = _loc.DefaultCulture;

        if (culture == null)
        {
            _sawmill.Error("Culture was null when trying to generate Ion Law");
            return;
        }

        _loc.AddFunction(culture, "ION-NUMBER-BASE", _ => GetIonLawValue("ION-NUMBER-BASE"));
        _loc.AddFunction(culture, "ION-NUMBER-MOD", _ => GetIonLawValue("ION-NUMBER-MOD"));
        _loc.AddFunction(culture, "ION-ADJECTIVE", _ => GetIonLawValue("ION-ADJECTIVE"));
        _loc.AddFunction(culture, "ION-SUBJECT", _ => GetIonLawValue("ION-SUBJECT"));
        _loc.AddFunction(culture, "ION-WHO", _ => GetIonLawValue("ION-WHO"));
        _loc.AddFunction(culture, "ION-MUST", _ => GetIonLawValue("ION-MUST"));
        _loc.AddFunction(culture, "ION-THING", _ => GetIonLawValue("ION-THING"));
        _loc.AddFunction(culture, "ION-JOB", _ => GetIonLawValue("ION-JOB"));
        _loc.AddFunction(culture, "ION-WHO-GENERAL", _ => GetIonLawValue("ION-WHO-GENERAL"));
        _loc.AddFunction(culture, "ION-PLURAL", _ => GetIonLawValue("ION-PLURAL"));
        _loc.AddFunction(culture, "ION-REQUIRE", _ => GetIonLawValue("ION-REQUIRE"));
        _loc.AddFunction(culture, "ION-SEVERITY", _ => GetIonLawValue("ION-SEVERITY"));
        _loc.AddFunction(culture, "ION-ALLERGY", _ => GetIonLawValue("ION-ALLERGY"));
        _loc.AddFunction(culture, "ION-FEELING", _ => GetIonLawValue("ION-FEELING"));
        _loc.AddFunction(culture, "ION-CONCEPT", _ => GetIonLawValue("ION-CONCEPT"));
        _loc.AddFunction(culture, "ION-FOOD", _ => GetIonLawValue("ION-FOOD"));
        _loc.AddFunction(culture, "ION-DRINK", _ => GetIonLawValue("ION-DRINK"));
        _loc.AddFunction(culture, "ION-CHANGE", _ => GetIonLawValue("ION-CHANGE"));
        _loc.AddFunction(culture, "ION-WHO-RANDOM", _ => GetIonLawValue("ION-WHO-RANDOM"));
        _loc.AddFunction(culture, "ION-AREA", _ => GetIonLawValue("ION-AREA"));
        _loc.AddFunction(culture, "ION-PART", _ => GetIonLawValue("ION-PART"));
        _loc.AddFunction(culture, "ION-OBJECT", _ => GetIonLawValue("ION-OBJECT"));
        _loc.AddFunction(culture, "ION-HARM-PROTECT", _ => GetIonLawValue("ION-HARM-PROTECT"));
        _loc.AddFunction(culture, "ION-VERB", _ => GetIonLawValue("ION-VERB"));
=======
        var culture = Loc.DefaultCulture;

        if (culture == null)
        {
            Log.Error("Culture was null when trying to generate Ion Law");
            return;
        }

        Loc.AddFunction(culture, "ION-NUMBER-BASE", _ => GetIonLawValue("ION-NUMBER-BASE"));
        Loc.AddFunction(culture, "ION-NUMBER-MOD", _ => GetIonLawValue("ION-NUMBER-MOD"));
        Loc.AddFunction(culture, "ION-ADJECTIVE", _ => GetIonLawValue("ION-ADJECTIVE"));
        Loc.AddFunction(culture, "ION-SUBJECT", _ => GetIonLawValue("ION-SUBJECT"));
        Loc.AddFunction(culture, "ION-WHO", _ => GetIonLawValue("ION-WHO"));
        Loc.AddFunction(culture, "ION-MUST", _ => GetIonLawValue("ION-MUST"));
        Loc.AddFunction(culture, "ION-THING", _ => GetIonLawValue("ION-THING"));
        Loc.AddFunction(culture, "ION-JOB", _ => GetIonLawValue("ION-JOB"));
        Loc.AddFunction(culture, "ION-WHO-GENERAL", _ => GetIonLawValue("ION-WHO-GENERAL"));
        Loc.AddFunction(culture, "ION-PLURAL", _ => GetIonLawValue("ION-PLURAL"));
        Loc.AddFunction(culture, "ION-REQUIRE", _ => GetIonLawValue("ION-REQUIRE"));
        Loc.AddFunction(culture, "ION-SEVERITY", _ => GetIonLawValue("ION-SEVERITY"));
        Loc.AddFunction(culture, "ION-ALLERGY", _ => GetIonLawValue("ION-ALLERGY"));
        Loc.AddFunction(culture, "ION-FEELING", _ => GetIonLawValue("ION-FEELING"));
        Loc.AddFunction(culture, "ION-CONCEPT", _ => GetIonLawValue("ION-CONCEPT"));
        Loc.AddFunction(culture, "ION-FOOD", _ => GetIonLawValue("ION-FOOD"));
        Loc.AddFunction(culture, "ION-DRINK", _ => GetIonLawValue("ION-DRINK"));
        Loc.AddFunction(culture, "ION-CHANGE", _ => GetIonLawValue("ION-CHANGE"));
        Loc.AddFunction(culture, "ION-WHO-RANDOM", _ => GetIonLawValue("ION-WHO-RANDOM"));
        Loc.AddFunction(culture, "ION-AREA", _ => GetIonLawValue("ION-AREA"));
        Loc.AddFunction(culture, "ION-PART", _ => GetIonLawValue("ION-PART"));
        Loc.AddFunction(culture, "ION-OBJECT", _ => GetIonLawValue("ION-OBJECT"));
        Loc.AddFunction(culture, "ION-HARM-PROTECT", _ => GetIonLawValue("ION-HARM-PROTECT"));
        Loc.AddFunction(culture, "ION-VERB", _ => GetIonLawValue("ION-VERB"));
>>>>>>> wizzden/master
    }

    /// <summary>
    /// Returns a localized value for an ion law token.
    /// </summary>
    private ILocValue GetIonLawValue(string s)
    {
        var val = _ionLaw.GetOrGenerateValue(s);
        return new LocValueString(val.ToString() ?? string.Empty);
    }
}
