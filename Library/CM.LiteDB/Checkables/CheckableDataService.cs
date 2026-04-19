namespace CM.LiteDB.Checkables;

/// <summary>
/// LiteDB implementation of <see cref="ICheckableDataService"/>.
/// </summary>
internal sealed class CheckableDataService : ICheckableDataService
{
    private const string CollectionName = "checkables";

    private readonly ILiteDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckableDataService"/> class.
    /// </summary>
    /// <param name="database">The LiteDB database instance.</param>
    public CheckableDataService(ILiteDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    private ILiteCollection<BsonDocument> Collection
    {
        get
        {
            var col = _database.GetCollection(CollectionName);
            col.EnsureIndex("checkListId");
            return col;
        }
    }

    /// <inheritdoc />
    public Task<Result<bool>> DeleteAsync(Guid checkableId)
    {
        var deleted = Collection.Delete(new BsonValue(checkableId));
        if (!deleted)
        {
            return Task.FromResult<Result<bool>>(new FailResult($"Checkable '{checkableId}' not found."));
        }

        return Task.FromResult<Result<bool>>(true);
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<Checkable>>> GetByCheckListAsync(Guid checkListId)
    {
        var results = Collection
            .Find(Query.EQ("checkListId", new BsonValue(checkListId)))
            .Select(ToCheckable)
            .ToList();

        return Task.FromResult(Result<IReadOnlyList<Checkable>>.Success(results));
    }

    /// <inheritdoc />
    public Task<Result<Checkable>> GetByIdAsync(Guid checkableId)
    {
        var doc = Collection.FindById(new BsonValue(checkableId));
        if (doc is null)
        {
            return Task.FromResult<Result<Checkable>>(new FailResult($"Checkable '{checkableId}' not found."));
        }

        return Task.FromResult<Result<Checkable>>(ToCheckable(doc));
    }

    /// <inheritdoc />
    public Task<Result<Checkable>> UpsertAsync(Checkable checkable)
    {
        Collection.Upsert(ToDocument(checkable));
        return Task.FromResult<Result<Checkable>>(checkable);
    }

    private static BsonDocument ToDocument(Checkable checkable) => new()
    {
        ["_id"] = new BsonValue(checkable.Id),
        ["checked"] = new BsonValue(checkable.Checked),
        ["checkListId"] = new BsonValue(checkable.CheckListId),
        ["created"] = new BsonValue(checkable.Created.ToString("O")),
        ["description"] = new BsonValue(checkable.Description),
        ["userId"] = new BsonValue(checkable.UserId)
    };

    private static Checkable ToCheckable(BsonDocument doc) => new(
        @checked: doc["checked"].AsBoolean,
        checkListId: doc["checkListId"].AsGuid,
        created: DateTimeOffset.Parse(doc["created"].AsString),
        description: doc["description"].AsString,
        id: doc["_id"].AsGuid,
        userId: doc["userId"].AsGuid);
}
