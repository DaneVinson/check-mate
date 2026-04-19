namespace CM.Domain.Users.Commands;

/// <summary>
/// Command to create a new user.
/// </summary>
/// <param name="Email">The email address of the new user.</param>
/// <param name="Name">The display name of the new user.</param>
public record CreateUser(string Email, string Name) : ICommand, IValidatable
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public FailResult? Validate()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            return new FailResult("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            return new FailResult("Name is required.");
        }

        return null;
    }
}
