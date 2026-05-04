using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Numerics;

namespace Content.Server._Utopia.ZLevels.Events;

public sealed class GridMotionChangedEvent : EntityEventArgs
{
    public Vector2 LinearDirection;
    public float LinearPower;
    public float AngularPower;
    public TimeSpan Time;
}
