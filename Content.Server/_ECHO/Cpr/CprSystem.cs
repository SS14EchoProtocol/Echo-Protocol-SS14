using Content.Shared._ECHO.Cpr;
using Robust.Shared.Player;

namespace Content.Server._ECHO.Cpr;

public sealed partial class CprSystem : SharedCprSystem
{
    public override void DoLunge(EntityUid user)
    {
        // raise event for all nearby players
        Filter filter = Filter.PvsExcept(user, entityManager: Ent);

        RaiseNetworkEvent(new CprLungeEvent(GetNetEntity(user)), filter);
    }
}
