namespace CM.Domain.Checkables.Events;

/// <summary>
/// Event raised when a checkable item's description has been updated.
/// </summary>
/// <param name="Checkable">The updated checkable item.</param>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
public record CheckableUpdated(CheckableDto Checkable, Guid? CommandId) : IEvent;
