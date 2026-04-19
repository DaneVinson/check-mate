namespace CM.Domain.Cqrs;

/// <summary>
/// Event raised when a command has failed to complete successfully.
/// </summary>
/// <param name="CommandId">The identifier of the command that failed, if any.</param>
/// <param name="Message">The message describing the failure.</param>
public record CommandFailed(Guid? CommandId, string Message) : IEvent;
