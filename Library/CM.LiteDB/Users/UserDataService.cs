namespace CM.LiteDB.Users;

/// <summary>
/// LiteDB implementation of <see cref="IUserDataService"/>.
/// </summary>
internal sealed class UserDataService : IUserDataService
{
    private const string CollectionName = "users";

    private readonly ILiteDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDataService"/> class.
    /// </summary>
    /// <param name="database">The LiteDB database instance.</param>
    public UserDataService(ILiteDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    private ILiteCollection<BsonDocument> Collection => _database.GetCollection(CollectionName);

    /// <inheritdoc />
    public Task<Result<User>> GetByIdAsync(Guid userId)
    {
        var doc = Collection.FindById(new BsonValue(userId));
        if (doc is null)
        {
            return Task.FromResult<Result<User>>(new FailResult($"User '{userId}' not found."));
        }

        return Task.FromResult<Result<User>>(ToUser(doc));
    }

    /// <inheritdoc />
    public Task<Result<User>> UpsertAsync(User user)
    {
        Collection.Upsert(ToDocument(user));
        return Task.FromResult<Result<User>>(user);
    }

    private static BsonDocument ToDocument(User user) => new()
    {
        ["_id"] = new BsonValue(user.Id),
        ["created"] = new BsonValue(user.Created.ToString("O")),
        ["email"] = new BsonValue(user.Email),
        ["name"] = new BsonValue(user.Name)
    };

    private static User ToUser(BsonDocument doc) => new(
        created: DateTimeOffset.Parse(doc["created"].AsString),
        email: doc["email"].AsString,
        id: doc["_id"].AsGuid,
        name: doc["name"].AsString);
}
