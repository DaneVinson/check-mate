namespace CM.Domain.Checkables;

/// <summary>
/// Represents a single checkable item within a <see cref="CheckList"/>.
/// </summary>
public sealed class Checkable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Checkable"/> class.
    /// </summary>
    /// <param name="checked">Whether the item has been checked off.</param>
    /// <param name="checkListId">The identifier of the check list this item belongs to.</param>
    /// <param name="created">The date and time the item was created.</param>
    /// <param name="description">The description of the item.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="userId">The identifier of the user who owns this item.</param>
    public Checkable(bool @checked, Guid checkListId, DateTimeOffset created, string description, Guid id, Guid userId)
    {
        Checked = @checked;
        CheckListId = checkListId;
        Created = created;
        Description = description;
        Id = id;
        UserId = userId;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this item has been checked off.
    /// </summary>
    public bool Checked { get; set; }

    /// <summary>
    /// Gets the identifier of the check list this item belongs to.
    /// </summary>
    public Guid CheckListId { get; init; }

    /// <summary>
    /// Gets the date and time the item was created.
    /// </summary>
    public DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets or sets the description of the item.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets the unique identifier of the item.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>
    /// Gets the identifier of the user who owns this item.
    /// </summary>
    public Guid UserId { get; init; }
}
