namespace CM.Domain.CheckLists.Events;

/// <summary>
/// Event raised when a check list's name has been updated.
/// </summary>
/// <param name="CheckList">The updated check list.</param>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
public record CheckListUpdated(CheckListDto CheckList, Guid? CommandId) : IEvent;
