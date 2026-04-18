namespace CM.Domain.CheckLists.Commands;

/// <summary>
/// Command to create a new check list.
/// </summary>
/// <param name="Name">The name of the new check list.</param>
/// <param name="UserId">The identifier of the user who will own this check list.</param>
public record CreateCheckList(string Name, Guid UserId) : ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
