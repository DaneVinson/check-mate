namespace CM.Domain.CheckLists.Commands;

/// <summary>
/// Command to delete an existing check list.
/// </summary>
/// <param name="CheckListId">The identifier of the check list to delete.</param>
public record DeleteCheckList(Guid CheckListId) : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
