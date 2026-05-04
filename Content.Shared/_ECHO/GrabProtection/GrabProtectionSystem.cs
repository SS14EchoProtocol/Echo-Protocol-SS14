using Content.Shared._ECHO.Grab;

namespace Content.Shared._ECHO.GrabProtection;

public sealed class GrabProtectionSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GrabProtectionComponent, ModifyGrabStageTimeEvent>(OnGrab);
    }

    private void OnGrab(EntityUid uid, GrabProtectionComponent component, ref ModifyGrabStageTimeEvent args)
    {
        args.Cancelled = true;
    }
}
