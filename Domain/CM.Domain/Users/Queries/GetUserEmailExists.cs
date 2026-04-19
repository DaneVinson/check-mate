namespace CM.Domain.Users.Queries;

/// <summary>
/// Query to determine whether a user with the specified email address exists.
/// </summary>
/// <param name="Email">The email address to check.</param>
public record GetUserEmailExists(string Email) : IQuery<bool>;
