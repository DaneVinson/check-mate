namespace CM.Domain.Users.Events;

/// <summary>
/// Event raised when a user's name has been updated.
/// </summary>
/// <param name="CommandId">The identifier of the command that originated this event, if any.</param>
/// <param name="User">The updated user.</param>
public record UserUpdated(Guid? CommandId, UserDto User) : IEvent;
