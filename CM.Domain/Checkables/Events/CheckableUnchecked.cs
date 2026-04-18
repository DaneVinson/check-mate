namespace CM.Domain.Checkables.Events;

/// <summary>
/// Event raised when a checkable item has been unchecked.
/// </summary>
/// <param name="Checkable">The checkable item that was unchecked.</param>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
public record CheckableUnchecked(CheckableDto Checkable, Guid? CommandId) : IEvent;
