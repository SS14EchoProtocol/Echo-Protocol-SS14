using System.Linq;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server.Spawners.Components;

// ECHO-Tweak Start
[Serializable]
[ImplicitDataDefinitionForInheritors]
public sealed partial class JobPrototypeWhitelistPack
{
    /// <summary>
    /// The job this spawn point is valid for.
    /// Null will allow all jobs to spawn here.
    /// </summary>
    [DataField("job_id")]
    public ProtoId<JobPrototype>? Job;
}
// ECHO-Tweak End

[RegisterComponent]
public sealed partial class SpawnPointComponent : Component, ISpawnPoint
{
    // ECHO-Tweak Start
    [ViewVariables(VVAccess.ReadOnly)]
    [DataField("whitelist")]
    public List<JobPrototypeWhitelistPack> WhitelistLate = new();
    // ECHO-Tweak End

    /// <summary>
    /// The job this spawn point is valid for.
    /// Null will allow all jobs to spawn here.
    /// </summary>
    [DataField("job_id")]
    public ProtoId<JobPrototype>? Job;
    /// <summary>
    /// The type of spawn point.
    /// </summary>
    [DataField("spawn_type"), ViewVariables(VVAccess.ReadWrite)]
    public SpawnPointType SpawnType { get; set; } = SpawnPointType.Unset;

    public override string ToString()
    {
        return $"{Job} {SpawnType}";
    }
}

public enum SpawnPointType
{
    Unset = 0,
    LateJoin,
    Job,
    Observer,
}
