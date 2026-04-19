namespace CM.Domain.Users.Queries;

/// <summary>
/// Query to retrieve a user by their unique identifier.
/// </summary>
/// <param name="UserId">The identifier of the user to retrieve.</param>
public record GetUser(Guid UserId) : IQuery<UserDto?>;
