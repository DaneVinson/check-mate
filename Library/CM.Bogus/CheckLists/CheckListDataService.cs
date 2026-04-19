namespace CM.Bogus.CheckLists;

/// <summary>
/// In-memory implementation of <see cref="ICheckListDataService"/> backed by <see cref="DataStore"/>.
/// </summary>
internal sealed class CheckListDataService : ICheckListDataService
{
    /// <inheritdoc />
    public Task<Result<bool>> DeleteAsync(Guid checkListId)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        var index = DataStore.CheckLists.FindIndex(cl => cl.Id == checkListId);
        if (index < 0)
        {
            return Task.FromResult<Result<bool>>(new FailResult($"CheckList '{checkListId}' not found."));
        }

        DataStore.CheckLists.RemoveAt(index);
        return Task.FromResult<Result<bool>>(true);
    }

    /// <inheritdoc />
    public Task<Result<CheckList>> GetByIdAsync(Guid checkListId)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        var checkList = DataStore.CheckLists.FirstOrDefault(cl => cl.Id == checkListId);
        if (checkList is null)
        {
            return Task.FromResult<Result<CheckList>>(new FailResult($"CheckList '{checkListId}' not found."));
        }

        return Task.FromResult<Result<CheckList>>(checkList);
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<CheckList>>> GetByUserAsync(Guid userId)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        return Task.FromResult(Result<IReadOnlyList<CheckList>>.Success(
            DataStore.CheckLists.Where(cl => cl.UserId == userId).ToList()));
    }

    /// <inheritdoc />
    public Task<Result<CheckList>> UpsertAsync(CheckList checkList)
    {
        using var scope = DataStore.SyncRoot.EnterScope();
        var index = DataStore.CheckLists.FindIndex(cl => cl.Id == checkList.Id);
        if (index >= 0)
        {
            DataStore.CheckLists[index] = checkList;
        }
        else
        {
            DataStore.CheckLists.Add(checkList);
        }

        return Task.FromResult<Result<CheckList>>(checkList);
    }
}
