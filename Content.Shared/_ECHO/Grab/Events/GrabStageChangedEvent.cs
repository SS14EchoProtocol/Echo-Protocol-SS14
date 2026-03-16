using Content.Shared.Movement.Pulling.Components;

namespace Content.Shared._ECHO.Grab;

[ByRefEvent]
public record struct GrabStageChangedEvent(Entity<PullerComponent> Puller, Entity<PullableComponent> Pulling, GrabStage OldStage, GrabStage NewStage);
