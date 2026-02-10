using Content.Echo.Server.Redial;
using Robust.Shared.IoC;

namespace Content.Echo.Server.IoC;

internal static class ServerEchoContentIoC
{
    internal static void Register()
    {
        var instance = IoCManager.Instance!;

        instance.Register<RedialManager>();
    }
}
