namespace CM.Domain.Users;

/// <summary>
/// Defines data access operations for <see cref="User"/> entities.
/// </summary>
public interface IUserDataService
{
    /// <summary>
    /// Returns a value indicating whether a <see cref="User"/> with the specified email address exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>A result containing <see langword="true"/> if a matching user exists; otherwise <see langword="false"/>.</returns>
    Task<Result<bool>> ExistsByEmailAsync(string email);

    /// <summary>
    /// Retrieves a <see cref="User"/> by email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>A result containing the matching user, or <see langword="null"/> if no user with the given email exists.</returns>
    Task<Result<User?>> GetByEmailAsync(string email);

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
