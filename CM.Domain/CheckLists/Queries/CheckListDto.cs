namespace CM.Domain.CheckLists.Queries;

/// <summary>
/// Represents the result of a check list query.
/// </summary>
/// <param name="Created">The date and time the check list was created.</param>
/// <param name="Id">The unique identifier of the check list.</param>
/// <param name="Name">The name of the check list.</param>
/// <param name="UserId">The identifier of the user who owns the check list.</param>
public record CheckListDto(DateTimeOffset Created, Guid Id, string Name, Guid UserId);
