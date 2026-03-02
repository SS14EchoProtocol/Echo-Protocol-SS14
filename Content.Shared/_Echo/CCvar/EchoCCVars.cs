using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

[CVarDefs]
public sealed partial class EchoCCVars
{
    public static readonly CVarDef<bool> FilmGrain =
        CVarDef.Create("postprocessing.grain_enabled", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    public static readonly CVarDef<float> FilmGrainAmount =
        CVarDef.Create("postprocessing.grain_amount", 0.07f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /*
    * Offer Items
    */
    public static readonly CVarDef<bool> OfferModeIndicatorsPointShow =
        CVarDef.Create("hud.offer_mode_indicators_point_show", true, CVar.ARCHIVE | CVar.CLIENTONLY);
}
