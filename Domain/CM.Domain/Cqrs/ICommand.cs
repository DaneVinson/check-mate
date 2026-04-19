namespace CM.Domain.Cqrs;

/// <summary>
/// Represents a command that can be dispatched in the CQRS pattern.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance.
    /// </summary>
    Guid Id { get; }
}
