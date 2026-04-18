namespace CM.Domain.Checkables.Commands;

/// <summary>
/// Command to delete an existing checkable item.
/// </summary>
/// <param name="CheckableId">The identifier of the checkable item to delete.</param>
public record DeleteCheckable(Guid CheckableId) : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
