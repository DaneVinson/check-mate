namespace CM.Domain.CheckLists.Queries;

/// <summary>
/// Query to retrieve all check lists belonging to a user.
/// </summary>
/// <param name="UserId">The identifier of the user whose check lists to retrieve.</param>
public record GetCheckListsByUser(Guid UserId) : IQuery<IReadOnlyList<CheckListDto>>;
