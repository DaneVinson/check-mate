namespace CM.Domain.CheckLists.Queries;

/// <summary>
/// Query to retrieve a check list by its unique identifier.
/// </summary>
/// <param name="CheckListId">The identifier of the check list to retrieve.</param>
public record GetCheckList(Guid CheckListId) : IQuery<CheckListDto?>;
