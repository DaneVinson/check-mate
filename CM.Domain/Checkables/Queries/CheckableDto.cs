namespace CM.Domain.Checkables.Queries;

/// <summary>
/// Represents the result of a checkable item query.
/// </summary>
/// <param name="Checked">Whether the item has been checked off.</param>
/// <param name="CheckListId">The identifier of the check list this item belongs to.</param>
/// <param name="Created">The date and time the item was created.</param>
/// <param name="Description">The description of the item.</param>
/// <param name="Id">The unique identifier of the item.</param>
/// <param name="UserId">The identifier of the user who owns this item.</param>
public record CheckableDto(bool Checked, Guid CheckListId, DateTimeOffset Created, string Description, Guid Id, Guid UserId)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckableDto"/> record from a <see cref="Checkable"/> entity.
    /// </summary>
    /// <param name="checkable">The checkable entity to map from.</param>
    public CheckableDto(Checkable checkable)
        : this(checkable.Checked, checkable.CheckListId, checkable.Created, checkable.Description, checkable.Id, checkable.UserId)
    {
    }
}
