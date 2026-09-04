using Content.Shared._ECHO.Emp;
using Content.Shared.Emp;

namespace Content.Server._ECHO.Emp;

public sealed class ScreenGlitchSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ScreenGlitchOnEmpComponent, EmpPulseEvent>(OnEmp);
    }

    private void OnEmp(Entity<ScreenGlitchOnEmpComponent> ent, ref EmpPulseEvent args)
    {
        var ev = new DoScreenGlitchMessage(ent.Comp.Offset, ent.Comp.Chroma, ent.Comp.Duration, ent.Comp.EffectSpeed, ent.Comp.Segments,
                                           ent.Comp.SeedUpdateInterval, ent.Comp.UpdateSeed);

        RaiseNetworkEvent(ev, ent.Owner);
    }
}
