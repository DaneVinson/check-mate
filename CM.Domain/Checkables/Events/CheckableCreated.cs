namespace CM.Domain.Checkables.Events;

/// <summary>
/// Event raised when a new checkable item has been created.
/// </summary>
/// <param name="Checkable">The created checkable item.</param>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
public record CheckableCreated(CheckableDto Checkable, Guid? CommandId) : IEvent;
