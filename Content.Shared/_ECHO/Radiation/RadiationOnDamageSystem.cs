using Content.Shared.Damage.Systems;
using Content.Shared.Radiation.Components;

namespace Content.Shared._ECHO.Radiation;

public sealed partial class RadiationOnDamageSystem : EntitySystem
{
    [Dependency] private DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RadiationOnDamageComponent, DamageChangedEvent>(OnDamageChanged);
    }

    private void OnDamageChanged(Entity<RadiationOnDamageComponent> ent, ref DamageChangedEvent args)
    {
        var radiation = EnsureComp<RadiationSourceComponent>(ent.Owner);
        var damage = _damageable.GetAllDamage(ent.Owner);

        if (damage.GetTotal() <= 0)
        {
            radiation.Intensity = 0;
            return;
        }

        var radiationIntensity = 0f;

        foreach (var item in ent.Comp.IntensityPerDamage)
        {
            if (!damage.DamageDict.TryGetValue(item.Key, out var typeDamage))
                continue;

            radiationIntensity = MathF.Min(radiationIntensity + (typeDamage.Float() * item.Value), ent.Comp.MaxIntensity);
        }

        radiation.Intensity = radiationIntensity;
    }
}
