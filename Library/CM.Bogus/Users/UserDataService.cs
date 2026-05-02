namespace CM.Bogus.Users;

/// <summary>
/// In-memory implementation of <see cref="IUserDataService"/> backed by <see cref="DataStore"/>.
/// </summary>
internal sealed class UserDataService : IUserDataService
{
    /// <inheritdoc />
    public Task<Result<bool>> ExistsByEmailAsync(string email)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        return Task.FromResult<Result<bool>>(DataStore.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }

    /// <inheritdoc />
    public Task<Result<User?>> GetByEmailAsync(string email)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        var user = DataStore.Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<Result<User?>>(user);
    }

    /// <inheritdoc />
    public Task<Result<User>> GetByIdAsync(Guid userId)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        var user = DataStore.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return Task.FromResult<Result<User>>(new FailResult($"User '{userId}' not found."));
        }

        return Task.FromResult<Result<User>>(user);
    }

    /// <inheritdoc />
    public Task<Result<User>> UpsertAsync(User user)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        var index = DataStore.Users.FindIndex(u => u.Id == user.Id);
        if (index >= 0)
        {
            DataStore.Users[index] = user;
        }
        else
        {
            DataStore.Users.Add(user);
        }

        return Task.FromResult<Result<User>>(user);
    }
}
