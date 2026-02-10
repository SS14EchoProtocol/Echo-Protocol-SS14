using Content.Echo.Server.IoC;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Content.Echo.Server.Entry;

public sealed class EntryPoint : GameServer
{
    public override void Init()
    {
        base.Init();

        ServerEchoContentIoC.Register();

        IoCManager.BuildGraph();
    }
}
