namespace CM.Domain.Users;

/// <summary>
/// Defines data access operations for <see cref="User"/> entities.
/// </summary>
public interface IUserDataService
{
    /// <summary>
    /// Retrieves a <see cref="User"/> by its unique identifier.
    /// </summary>
    /// <param name="userId">The identifier of the user to retrieve.</param>
    /// <returns>A result containing the matching user.</returns>
    Task<Result<User>> GetByIdAsync(Guid userId);

    /// <summary>
    /// Inserts or updates a <see cref="User"/>.
    /// </summary>
    /// <param name="user">The user to insert or update.</param>
    /// <returns>A result containing the upserted user.</returns>
    Task<Result<User>> UpsertAsync(User user);
}
