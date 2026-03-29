using System.Numerics;
using Content.Server.Administration;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Decals;
using Content.Server.Parallax;
using Content.Shared.Administration;
using Content.Shared.Atmos;
using Content.Shared.Light.Components;
using Content.Shared.Parallax.Biomes;
using Robust.Server.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._ECHO.Procedural;

public sealed class BiomePreloadSystem : EntitySystem
{
    [Dependency] private readonly IConsoleHost _console = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly AtmosphereSystem _atmos = default!;
    [Dependency] private readonly BiomeSystem _biome = default!;
    [Dependency] private readonly DecalSystem _decal = default!;
    [Dependency] private readonly MapSystem _map = default!;

    public override void Initialize()
    {
        base.Initialize();

        _console.RegisterCommand("planet-limited",
            Loc.GetString("cmd-planet-limited-desc"),
            Loc.GetString("cmd-planet-limited-help"),
            GenerateCommand,
            CommandCompletion);
    }

    [AdminCommand(AdminFlags.Mapping)]
    private void GenerateCommand(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 4)
        {
            shell.WriteError(Loc.GetString("cmd-planet-limited-arguments"));
            return;
        }

        if (!int.TryParse(args[0], out var mapInt))
        {
            shell.WriteError(Loc.GetString($"cmd-planet-map", ("map", mapInt)));
            return;
        }

        var mapId = new MapId(mapInt);
        if (!_map.MapExists(mapId))
        {
            shell.WriteError(Loc.GetString($"cmd-planet-map", ("map", mapId)));
            return;
        }

        var mapUid = _map.GetMapOrInvalid(mapId);

        if (!_proto.TryIndex<BiomeTemplatePrototype>(args[1], out var proto))
        {
            shell.WriteError(Loc.GetString($"cmd-planet-map-prototype", ("prototype", args[1])));
            return;
        }

        if (!int.TryParse(args[2], out var sizeX))
        {
            shell.WriteError(Loc.GetString($"cmd-parse-failure-integer", ("prototype", args[1])));
            return;
        }

        if (!int.TryParse(args[3], out var sizeY))
        {
            shell.WriteError(Loc.GetString($"cmd-parse-failure-integer", ("prototype", args[1])));
            return;
        }

        string? limitingEnt = args[4];

        if (args[4] == "null")
            limitingEnt = null;

        Vector2i pos = new(0, 0);

        if (int.TryParse(args[5], out var x))
            pos.X = x;

        if (!int.TryParse(args[6], out var y))
            pos.Y = y;

        CreateAndGenerate(mapUid, new(sizeX, sizeY), pos, args[1], limitingEnt);
        shell.WriteLine(Loc.GetString($"cmd-planet-limited-success", ("mapId", args[0])));
    }

    private CompletionResult CommandCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
            return CompletionResult.FromHintOptions(CompletionHelper.MapIds(EntityManager), "Map Id");

        if (args.Length == 2)
            return CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<BiomeTemplatePrototype>(), "Biome template");

        if (args.Length == 3)
            return CompletionResult.FromHint("X scale");

        if (args.Length == 4)
            return CompletionResult.FromHint("Y scale");

        if (args.Length == 5)
            return CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<EntityPrototype>(), "Border entity");

        if (args.Length == 6)
            return CompletionResult.FromHint("X offset");

        if (args.Length == 7)
            return CompletionResult.FromHint("Y offset");

        return CompletionResult.Empty;
    }

    public void CreateAndGenerate(EntityUid mapUid,
                                  Vector2i scale,
                                  Vector2i offset,
                                  ProtoId<BiomeTemplatePrototype> biomeTemplate,
                                  string? limitingEntity = null,
                                  int? seed = null,
                                  Color? mapLight = null)
    {
        EnsureComp<MapGridComponent>(mapUid);

        var comp = EnsureComp<BiomePreloadComponent>(mapUid);
        comp.Biome = biomeTemplate;
        comp.LoadedBox = scale;

        if (comp.Seed == 0)
            comp.Seed = seed ?? _random.Next();

        Generate(mapUid, offset, limitingEntity);

        // Day lighting
        // Daylight: #D8B059
        // Midday: #E6CB8B
        // Moonlight: #2b3143
        // Lava: #A34931
        var light = EnsureComp<MapLightComponent>(mapUid);
        light.AmbientLightColor = mapLight ?? Color.FromHex("#D8B059");
        Dirty(mapUid, light);

        EnsureComp<RoofComponent>(mapUid);

        EnsureComp<LightCycleComponent>(mapUid);

        EnsureComp<SunShadowComponent>(mapUid);
        EnsureComp<SunShadowCycleComponent>(mapUid);

        var moles = new float[Atmospherics.AdjustedNumberOfGases];
        moles[(int)Gas.Oxygen] = 21.824779f;
        moles[(int)Gas.Nitrogen] = 82.10312f;

        var mixture = new GasMixture(moles, Atmospherics.T20C);

        _atmos.SetMapAtmosphere(mapUid, false, mixture);
    }

    public void Generate(Entity<BiomePreloadComponent?> ent, Vector2i pos, string? limitingEntity = null)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, false))
            return;

        if (!TryComp<MapGridComponent>(ent.Owner, out var grid))
            return;

        var proto = _proto.Index(ent.Comp.Biome);

        for (int x = -ent.Comp.LoadedBox.X + pos.X; x <= ent.Comp.LoadedBox.X + pos.X; x++)
        {
            for (int y = -ent.Comp.LoadedBox.Y + pos.Y; y <= ent.Comp.LoadedBox.Y + pos.Y; y++)
            {
                TryApplyBiome((ent.Owner, grid), proto, new Vector2i(x, y), ent.Comp.Seed);

                if (limitingEntity != null && (MathF.Abs(x) == ent.Comp.LoadedBox.X + pos.X || MathF.Abs(y) == ent.Comp.LoadedBox.Y + pos.Y))
                    Spawn(limitingEntity, new EntityCoordinates(ent.Owner, x, y));
            }
        }
    }

    private void TryApplyBiome(Entity<MapGridComponent> grid, BiomeTemplatePrototype biome, Vector2i pos, int seed)
    {
        if (_map.TryGetTile(grid, pos, out var existingTile) && !existingTile.IsEmpty)
            return;

        if (_biome.TryGetTile(pos, biome.Layers, seed, null, out var tile))
        {
            _map.SetTile(grid, pos, tile.Value);
            if (_biome.TryGetEntity(pos, biome.Layers, tile.Value, seed, grid, out var entity))
                Spawn(entity, new EntityCoordinates(grid.Owner, pos));

            if (_biome.TryGetDecals(pos, biome.Layers, seed, grid, out var decals))
            {
                foreach (var decal in decals)
                    _decal.TryAddDecal(decal.ID, new(grid.Owner, decal.Position), out _);
            }
        }
    }
}
