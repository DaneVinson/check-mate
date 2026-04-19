namespace CM.Domain.Checkables.Events;

/// <summary>
/// Event raised when a checkable item has been deleted.
/// </summary>
/// <param name="CheckableId">The identifier of the deleted checkable item.</param>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
public record CheckableDeleted(Guid CheckableId, Guid? CommandId) : IEvent;
