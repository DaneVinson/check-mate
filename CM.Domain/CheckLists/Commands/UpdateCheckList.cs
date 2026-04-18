namespace CM.Domain.CheckLists.Commands;

/// <summary>
/// Command to update the name of an existing check list.
/// </summary>
/// <param name="CheckListId">The identifier of the check list to update.</param>
/// <param name="Name">The new name for the check list.</param>
public record UpdateCheckList(Guid CheckListId, string Name) : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
