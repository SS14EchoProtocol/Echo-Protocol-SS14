using System.Linq;
using Content.Server.Administration;
using Content.Shared._Utopia.ZLevels.Components;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;

namespace Content.Server._Utopia.ZLevels;

[AdminCommand(AdminFlags.Server | AdminFlags.Mapping)]
public sealed class SaveGridZNetworkCommand : LocalizedEntityCommands
{
    [Dependency] private readonly IEntityManager _entities = default!;
    [Dependency] private readonly IResourceManager _resMan = default!;
    [Dependency] private readonly ZNetworkMappingSystem _zLoader = default!;

    public override string Command => "znetwork-savegrid";
    public override string Description => "Save all zNetwork linked grids";

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = new List<CompletionOption>();
            var query = _entities.EntityQueryEnumerator<GridMotionLinkComponent, MetaDataComponent>();
            while (query.MoveNext(out var uid, out _, out var meta))
            {
                options.Add(new CompletionOption(_entities.GetNetEntity(uid).ToString(), meta.EntityName));
            }
            return CompletionResult.FromHintOptions(options, "zNetwork net entity");
        }
        if (args.Length == 2)
        {
            var opts = CompletionHelper.UserFilePath(args[1], _resMan.UserData)
                .Concat(CompletionHelper.ContentFilePath(args[1], _resMan));

            return CompletionResult.FromHintOptions(opts, Loc.GetString("cmd-hint-savemap-path"));
        }
        return CompletionResult.Empty;
    }

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteError("Wrong arguments count.");
            return;
        }

        // get the target
        EntityUid? target;

        if (!NetEntity.TryParse(args[0], out var targetNet) ||
            !_entities.TryGetEntity(targetNet, out target))
        {
            shell.WriteError($"Unable to find entity {args[0]}");
            return;
        }

        if (!_zLoader.TrySaveGrid(args[1], target.Value, out var error))
        {
            shell.WriteError(error);
        }
    }
}
