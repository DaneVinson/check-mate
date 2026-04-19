namespace CM.Bogus;

/// <summary>
/// Provides deterministic seed data for development and testing.
/// </summary>
public static class DataSeed
{
    private const int Seed = 20250417;

    private static readonly IReadOnlyList<Checkable> _checkables;
    private static readonly IReadOnlyList<CheckList> _checkLists;
    private static readonly IReadOnlyList<User> _users;

    static DataSeed()
    {
        _users = new List<User>
        {
            new User(
                created: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                email: "bilbo.baggins@shire.me",
                id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                name: "Bilbo Baggins")
        }.AsReadOnly();

        var userId = _users[0].Id;

        var checkableFaker = new Faker<Checkable>()
            .UseSeed(Seed)
            .CustomInstantiator(f => new Checkable(
                @checked: f.Random.Bool(),
                checkListId: Guid.Empty, // overwritten per list below
                created: f.Date.PastOffset(365),
                description: f.Lorem.Sentence(),
                id: f.Random.Guid(),
                userId: userId));

        var checkListFaker = new Faker<CheckList>()
            .UseSeed(Seed)
            .CustomInstantiator(f => new CheckList(
                created: f.Date.PastOffset(365),
                id: f.Random.Guid(),
                name: f.Commerce.ProductName(),
                userId: userId));

        _checkLists = checkListFaker.Generate(10).AsReadOnly();

        var countFaker = new Faker { Random = new Randomizer(Seed) };

        _checkables = _checkLists
            .SelectMany(list =>
            {
                var itemCount = countFaker.Random.Int(3, 20);
                return checkableFaker.Generate(itemCount)
                    .Select(c => new Checkable(c.Checked, list.Id, c.Created, c.Description, c.Id, c.UserId));
            })
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Returns all seeded <see cref="Checkable"/> items across all check lists.
    /// </summary>
    public static IReadOnlyList<Checkable> GetCheckables() => _checkables;

    /// <summary>
    /// Returns all seeded <see cref="CheckList"/> instances.
    /// </summary>
    public static IReadOnlyList<CheckList> GetCheckLists() => _checkLists;

    /// <summary>
    /// Returns all seeded <see cref="User"/> instances.
    /// </summary>
    public static IReadOnlyList<User> GetUsers() => _users;
}
