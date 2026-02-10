using Robust.Shared.IoC;

namespace Content.Echo.Shared.IoC;

internal static class SharedEchoContentIoC
{
    internal static void Register()
    {
        var instance = IoCManager.Instance!;
    }
}
