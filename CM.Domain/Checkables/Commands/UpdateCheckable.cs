namespace CM.Domain.Checkables.Commands;

/// <summary>
/// Command to update the description of an existing checkable item.
/// </summary>
/// <param name="CheckableId">The identifier of the checkable item to update.</param>
/// <param name="Description">The new description for the checkable item.</param>
public record UpdateCheckable(Guid CheckableId, string Description) : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
