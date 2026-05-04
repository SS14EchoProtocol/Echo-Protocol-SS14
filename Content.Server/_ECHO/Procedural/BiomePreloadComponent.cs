using System.Numerics;
using Content.Server.Worldgen.Prototypes;
using Content.Shared.Parallax.Biomes;
using Robust.Shared.Prototypes;

namespace Content.Server._ECHO.Procedural;

[RegisterComponent]
public sealed partial class BiomePreloadComponent : Component
{
    [DataField(required: true), ViewVariables(VVAccess.ReadWrite)]
    public Vector2i LoadedBox = Vector2i.Zero;

    [DataField(required: true), ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<BiomeTemplatePrototype> Biome;

    [ViewVariables(VVAccess.ReadWrite)]
    public int Seed = 0;
}
