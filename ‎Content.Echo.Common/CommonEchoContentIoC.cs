using Robust.Shared.IoC;

namespace Content.Echo.Common.IoC;

internal static class CommonEchoContentIoC
{
    internal static void Register()
    {
        var instance = IoCManager.Instance!;
    }
}
