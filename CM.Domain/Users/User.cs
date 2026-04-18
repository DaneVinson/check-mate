namespace CM.Domain.Users;

/// <summary>
/// Represents a user of the CheckMate application.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="created">The date and time the user was created.</param>
    /// <param name="email">The email address of the user.</param>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="name">The display name of the user.</param>
    public User(DateTimeOffset created, string email, Guid id, string name)
    {
        Created = created;
        Email = email;
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets the date and time the user was created.
    /// </summary>
    public DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the email address of the user.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// Gets the unique identifier of the user.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>
    /// Gets or sets the display name of the user.
    /// </summary>
    public string Name { get; set; }
}
