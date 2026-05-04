using System.Linq;
using System.Numerics;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;

namespace Content.Server._Utopia.ZLevels;

[AdminCommand(AdminFlags.Server | AdminFlags.Mapping)]
public sealed class LoadGridZNetworkCommand : LocalizedEntityCommands
{
    [Dependency] private readonly IResourceManager _resMan = default!;
    [Dependency] private readonly ZNetworkMappingSystem _zLoader = default!;

    public override string Command => "loadgrid-znetwork";
    public override string Description => "Load your ZNetwork grid";

    public static CompletionResult GetCompletionResult(IConsoleShell shell, string[] args, IResourceManager resource, ILocalizationManager loc)
    {
        switch (args.Length)
        {
            case 1:
                return CompletionResult.FromHint(loc.GetString("cmd-hint-savemap-id"));
            case 2:
                var opts = CompletionHelper.UserFilePath(args[1], resource.UserData)
                    .Concat(CompletionHelper.ContentFilePath(args[1], resource));
                return CompletionResult.FromHintOptions(opts, loc.GetString("cmd-hint-savemap-path"));
            case 3:
                return CompletionResult.FromHint(loc.GetString("cmd-hint-loadmap-x-position"));
            case 4:
                return CompletionResult.FromHint(loc.GetString("cmd-hint-loadmap-y-position"));
            case 5:
                return CompletionResult.FromHint(loc.GetString("cmd-hint-loadmap-rotation"));
        }

        return CompletionResult.Empty;
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return GetCompletionResult(shell, args, _resMan, Loc);
    }

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2 || args.Length > 6)
        {
            shell.WriteLine(Help);
            return;
        }

        if (!int.TryParse(args[0], out var intMapId))
        {
            shell.WriteError(Loc.GetString("cmd-parse-failure-integer", ("arg", args[0])));
            return;
        }

        var mapId = new MapId(intMapId);

        // no loading null space
        if (mapId == MapId.Nullspace)
        {
            shell.WriteError(Loc.GetString("cmd-loadmap-nullspace"));
            return;
        }

        var offset = Vector2.Zero;

        if (float.TryParse(args[2], out var x))
            offset.X = x;

        if (float.TryParse(args[3], out var y))
            offset.Y = y;

        float.TryParse(args[4], out var rotation);

        if (_zLoader.TryLoadGrid(args[1], mapId, offset, rotation, out var error))
            shell.WriteLine(Loc.GetString("cmd-loadmap-success", ("mapId", mapId), ("path", args[1])));
        else
            shell.WriteLine(error);
    }
}
