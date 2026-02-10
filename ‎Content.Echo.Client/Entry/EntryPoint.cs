using Content.Echo.Client.IoC;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Content.Echo.Client.Entry;

public sealed class EntryPoint : GameClient
{
    public override void PreInit()
    {
        base.PreInit();
    }

    public override void Init()
    {
        ContentEchoClientIoC.Register();

        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
    }
}
