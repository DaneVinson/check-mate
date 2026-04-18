namespace CM.Domain.Checkables.Events;

/// <summary>
/// Event raised when a checkable item has been checked.
/// </summary>
/// <param name="Checkable">The checkable item that was checked.</param>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
public record CheckableChecked(CheckableDto Checkable, Guid? CommandId) : IEvent;
