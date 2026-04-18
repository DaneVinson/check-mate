namespace CM.Domain.Checkables.Commands;

/// <summary>
/// Command to create a new checkable item within a check list.
/// </summary>
/// <param name="CheckListId">The identifier of the check list this item belongs to.</param>
/// <param name="Description">The description of the new checkable item.</param>
/// <param name="UserId">The identifier of the user who owns this item.</param>
public record CreateCheckable(Guid CheckListId, string Description, Guid UserId) : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
