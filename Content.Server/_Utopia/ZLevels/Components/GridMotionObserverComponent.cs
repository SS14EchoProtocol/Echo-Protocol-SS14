using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Numerics;

namespace Content.Server._Utopia.ZLevels.Components;

[RegisterComponent]
public sealed partial class GridMotionObserverComponent : Component
{
    public Vector2 LastLinearVelocity;
    public float LastAngularVelocity;
    public bool SuppressNextTick;
}
