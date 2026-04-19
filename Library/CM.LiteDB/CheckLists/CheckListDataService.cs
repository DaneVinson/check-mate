namespace CM.LiteDB.CheckLists;

/// <summary>
/// LiteDB implementation of <see cref="ICheckListDataService"/>.
/// </summary>
internal sealed class CheckListDataService : ICheckListDataService
{
    private const string CollectionName = "checklists";

    private readonly ILiteDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckListDataService"/> class.
    /// </summary>
    /// <param name="database">The LiteDB database instance.</param>
    public CheckListDataService(ILiteDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    private ILiteCollection<BsonDocument> Collection
    {
        get
        {
            var col = _database.GetCollection(CollectionName);
            col.EnsureIndex("userId");
            return col;
        }
    }

    /// <inheritdoc />
    public Task<Result<bool>> DeleteAsync(Guid checkListId)
    {
        var deleted = Collection.Delete(new BsonValue(checkListId));
        if (!deleted)
        {
            return Task.FromResult<Result<bool>>(new FailResult($"CheckList '{checkListId}' not found."));
        }

        return Task.FromResult<Result<bool>>(true);
    }

    /// <inheritdoc />
    public Task<Result<CheckList>> GetByIdAsync(Guid checkListId)
    {
        var doc = Collection.FindById(new BsonValue(checkListId));
        if (doc is null)
        {
            return Task.FromResult<Result<CheckList>>(new FailResult($"CheckList '{checkListId}' not found."));
        }

        return Task.FromResult<Result<CheckList>>(ToCheckList(doc));
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<CheckList>>> GetByUserAsync(Guid userId)
    {
        var results = Collection
            .Find(Query.EQ("userId", new BsonValue(userId)))
            .Select(ToCheckList)
            .ToList();

        return Task.FromResult(Result<IReadOnlyList<CheckList>>.Success(results));
    }

    /// <inheritdoc />
    public Task<Result<CheckList>> UpsertAsync(CheckList checkList)
    {
        Collection.Upsert(ToDocument(checkList));
        return Task.FromResult<Result<CheckList>>(checkList);
    }

    private static BsonDocument ToDocument(CheckList checkList) => new()
    {
        ["_id"] = new BsonValue(checkList.Id),
        ["created"] = new BsonValue(checkList.Created.ToString("O")),
        ["name"] = new BsonValue(checkList.Name),
        ["userId"] = new BsonValue(checkList.UserId)
    };

    private static CheckList ToCheckList(BsonDocument doc) => new(
        created: DateTimeOffset.Parse(doc["created"].AsString),
        id: doc["_id"].AsGuid,
        name: doc["name"].AsString,
        userId: doc["userId"].AsGuid);
}
