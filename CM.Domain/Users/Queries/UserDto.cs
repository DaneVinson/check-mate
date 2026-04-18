namespace CM.Domain.Users.Queries;

/// <summary>
/// Represents the result of a user query.
/// </summary>
/// <param name="Created">The date and time the user was created.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Name">The display name of the user.</param>
public record UserDto(DateTimeOffset Created, string Email, Guid Id, string Name);
