using Content.Server._Utopia.ZLevels.Components;
using Content.Server._Utopia.ZLevels.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Maths;

namespace Content.Server._Utopia.ZLevels.Systems;

public sealed class GridThrustSystem : EntitySystem
{
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;

    public void Apply(EntityUid grid, GridMotionCommandEvent ev)
    {
        if (!TryComp(grid, out GridMotionObserverComponent? observer))
            return;

        observer.SuppressNextTick = true;

        _physics.SetLinearVelocity(
            grid,
            ev.LinearDirection * ev.LinearPower);

        _physics.SetAngularVelocity(
            grid,
            ev.AngularPower);
    }
}
