using System.Numerics;

namespace Content.Shared._ECHO.Extensions;

public static class VectorExtensions
{
    public static Vector2 MoveTowards(Vector2 from, Vector2 to, float maxMagnitude)
    {
        if (maxMagnitude <= 0)
            return from;

        if (Vector2.Distance(from, to) <= maxMagnitude)
            return to;

        var change = (to - from).Normalized() * maxMagnitude;
        return from + change;
    }
}
