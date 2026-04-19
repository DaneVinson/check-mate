namespace CM.Domain.CheckLists.Events;

/// <summary>
/// Event raised when a new check list has been created.
/// </summary>
/// <param name="CheckList">The created check list.</param>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
public record CheckListCreated(CheckListDto CheckList, Guid? CommandId) : IEvent;
