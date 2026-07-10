using System.Numerics;

namespace Content.Shared._ECHO.Extensions;

public static class MathExtensions
{
    public static float MoveTowards(float from, float to, float maxMagnitude)
    {
        if (maxMagnitude <= 0)
            return from;

        var diff = from - to;

        if (MathF.Abs(diff) <= maxMagnitude)
            return to;

        var change = diff * maxMagnitude;
        return from + change;
    }
}
