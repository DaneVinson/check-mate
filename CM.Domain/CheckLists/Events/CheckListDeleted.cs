namespace CM.Domain.CheckLists.Events;

/// <summary>
/// Event raised when a check list has been deleted.
/// </summary>
/// <param name="CheckListId">The identifier of the deleted check list.</param>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
public record CheckListDeleted(Guid CheckListId, Guid? CommandId) : IEvent;
