namespace CM.Domain.Users.Commands;

/// <summary>
/// Command to create a new user.
/// </summary>
/// <param name="Email">The email address of the new user.</param>
/// <param name="Name">The display name of the new user.</param>
public record CreateUser(string Email, string Name) : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
