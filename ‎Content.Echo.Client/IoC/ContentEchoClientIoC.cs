using Content.Echo.Client.Redial;
using Robust.Shared.IoC;

namespace Content.Echo.Client.IoC;

internal static class ContentEchoClientIoC
{
    internal static void Register()
    {
        var collection = IoCManager.Instance!;

        collection.Register<RedialManager>();
    }
}
