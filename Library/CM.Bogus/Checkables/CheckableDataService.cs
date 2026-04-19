namespace CM.Bogus.Checkables;

/// <summary>
/// In-memory implementation of <see cref="ICheckableDataService"/> backed by <see cref="DataStore"/>.
/// </summary>
internal sealed class CheckableDataService : ICheckableDataService
{
    /// <inheritdoc />
    public Task<Result<bool>> DeleteAsync(Guid checkableId)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        var index = DataStore.Checkables.FindIndex(c => c.Id == checkableId);
        if (index < 0)
        {
            return Task.FromResult<Result<bool>>(new FailResult($"Checkable '{checkableId}' not found."));
        }

        DataStore.Checkables.RemoveAt(index);
        return Task.FromResult<Result<bool>>(true);
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<Checkable>>> GetByCheckListAsync(Guid checkListId)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        return Task.FromResult(Result<IReadOnlyList<Checkable>>.Success(
            DataStore.Checkables.Where(c => c.CheckListId == checkListId).ToList()));
    }

    /// <inheritdoc />
    public Task<Result<Checkable>> GetByIdAsync(Guid checkableId)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        var checkable = DataStore.Checkables.FirstOrDefault(c => c.Id == checkableId);
        if (checkable is null)
        {
            return Task.FromResult<Result<Checkable>>(new FailResult($"Checkable '{checkableId}' not found."));
        }

        return Task.FromResult<Result<Checkable>>(checkable);
    }

    /// <inheritdoc />
    public Task<Result<Checkable>> UpsertAsync(Checkable checkable)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        var index = DataStore.Checkables.FindIndex(c => c.Id == checkable.Id);
        if (index >= 0)
        {
            DataStore.Checkables[index] = checkable;
        }
        else
        {
            DataStore.Checkables.Add(checkable);
        }

        return Task.FromResult<Result<Checkable>>(checkable);
    }
}
