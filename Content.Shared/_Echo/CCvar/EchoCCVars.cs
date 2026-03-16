using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

[CVarDefs]
public sealed partial class EchoCCVars
{
    /**
     * Post-Processing
     */

    public static readonly CVarDef<bool> FilmGrain =
        CVarDef.Create("postprocessing.grain_enabled", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    public static readonly CVarDef<float> FilmGrainAmount =
        CVarDef.Create("postprocessing.grain_amount", 0.07f, CVar.CLIENTONLY | CVar.ARCHIVE);

    /**
     * ZLevels
     */

    public static readonly CVarDef<float> ZImpactVelocityLimit =
        CVarDef.Create("zlevels.impact_velocity_limit", 0.75f, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

    public static readonly CVarDef<int> MaxZLevelsBelowRendering =
        CVarDef.Create("zlevels.max_z_levels_below_rendering", 3, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);

    public static readonly CVarDef<float> ZLevelOffset =
        CVarDef.Create("zlevels.z_level_offset", .2f, CVar.SERVER | CVar.REPLICATED | CVar.ARCHIVE);
    /*
    * Offer Items
    */
    public static readonly CVarDef<bool> OfferModeIndicatorsPointShow =
        CVarDef.Create("hud.offer_mode_indicators_point_show", true, CVar.ARCHIVE | CVar.CLIENTONLY);
}
