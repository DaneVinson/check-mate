namespace CM.Domain.Users.Commands;

/// <summary>
/// Command to update an existing user's name.
/// </summary>
/// <param name="Name">The new display name for the user.</param>
/// <param name="UserId">The identifier of the user to update.</param>
public record UpdateUser(string Name, Guid UserId) : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
