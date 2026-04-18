namespace CM.Domain.Users.Events;

/// <summary>
/// Event raised when a new user has been created.
/// </summary>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
/// <param name="User">The created user.</param>
public record UserCreated(Guid? CommandId, UserDto User) : IEvent;
