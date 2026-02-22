using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

[CVarDefs]
public sealed partial class EchoCCVars
{
    public static readonly CVarDef<bool> FilmGrain =
        CVarDef.Create("postprocessing.grain_enabled", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    public static readonly CVarDef<float> FilmGrainAmount =
        CVarDef.Create("postprocessing.grain_amount", 0.07f, CVar.CLIENTONLY | CVar.ARCHIVE);
}
