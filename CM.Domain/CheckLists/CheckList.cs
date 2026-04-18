namespace CM.Domain.CheckLists;

/// <summary>
/// Represents a named list of <see cref="Checkable"/> items belonging to a user.
/// </summary>
public sealed class CheckList
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckList"/> class.
    /// </summary>
    /// <param name="created">The date and time the check list was created.</param>
    /// <param name="id">The unique identifier of the check list.</param>
    /// <param name="name">The name of the check list.</param>
    /// <param name="userId">The identifier of the user who owns this check list.</param>
    public CheckList(DateTimeOffset created, Guid id, string name, Guid userId)
    {
        Created = created;
        Id = id;
        Name = name;
        UserId = userId;
    }

    /// <summary>
    /// Gets the date and time the check list was created.
    /// </summary>
    public DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the unique identifier of the check list.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>
    /// Gets or sets the name of the check list.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets the identifier of the user who owns this check list.
    /// </summary>
    public Guid UserId { get; init; }
}
