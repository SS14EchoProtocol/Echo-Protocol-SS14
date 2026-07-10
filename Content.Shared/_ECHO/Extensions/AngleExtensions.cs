using System.Numerics;

namespace Content.Shared._ECHO.Extensions;

public static class AngleExtensions
{
    public static Angle MoveTowards(Angle from, Angle to, float maxMagnitude)
    {
        if (maxMagnitude <= 0)
            return from;

        var diff = to.Degrees - from.Degrees;

        if (diff > 180)
            diff -= 360;
        else if (diff < -180)
            diff += 360;

        if (Math.Abs(diff) <= maxMagnitude)
            return to;

        return from + Angle.FromDegrees(diff * maxMagnitude);
    }
}
