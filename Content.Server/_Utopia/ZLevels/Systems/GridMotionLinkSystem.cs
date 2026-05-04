using Content.Shared._Utopia.ZLevels.Components;
using Content.Shared._Utopia.ZLevels.Systems;

namespace Content.Server._Utopia.ZLevels.Systems;

public sealed class GridMotionLinkSystem : SharedGridMotionLinkSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GridMotionLinkComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<GridMotionLinkComponent, GridFixtureChangeEvent>(OnGridFixtureChange);
    }

    private void OnMapInit(Entity<GridMotionLinkComponent> ent, ref MapInitEvent args)
    {
        UpdateOffset(ent);
        Dirty(ent);
    }

    private void OnGridFixtureChange(Entity<GridMotionLinkComponent> ent, ref GridFixtureChangeEvent args)
    {
        UpdateOffset(ent);
        Dirty(ent);
    }
}
