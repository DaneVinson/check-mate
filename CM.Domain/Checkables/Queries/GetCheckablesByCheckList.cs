namespace CM.Domain.Checkables.Queries;

/// <summary>
/// Query to retrieve all checkable items belonging to a check list.
/// </summary>
/// <param name="CheckListId">The identifier of the check list whose items to retrieve.</param>
public record GetCheckablesByCheckList(Guid CheckListId) : IQuery<IReadOnlyList<CheckableDto>>;
