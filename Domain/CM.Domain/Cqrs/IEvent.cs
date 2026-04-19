namespace CM.Domain.Cqrs;

/// <summary>
/// Represents a domain event raised as a result of a command.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Gets the identifier of the command that originated this event, if any.
    /// </summary>
    Guid? CommandId { get; }
}
