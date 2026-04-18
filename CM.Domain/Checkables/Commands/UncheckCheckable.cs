namespace CM.Domain.Checkables.Commands;

/// <summary>
/// Command to mark a checkable item as unchecked.
/// </summary>
/// <param name="CheckableId">The identifier of the checkable item to uncheck.</param>
public record UncheckCheckable(Guid CheckableId) : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
