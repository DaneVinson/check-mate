namespace CM.LiteDB.Tests.Users;

public sealed class UserDataServiceTests : IDisposable
{
    private readonly LiteDatabase _database = new(":memory:");
    private readonly ServiceProvider _provider;
    private readonly IUserDataService _sut;

    public UserDataServiceTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILiteDatabase>(_database);
        services.AddLiteDbDataServices();
        _provider = services.BuildServiceProvider();
        _sut = _provider.CreateScope().ServiceProvider.GetRequiredService<IUserDataService>();
    }

    public void Dispose()
    {
        _provider.Dispose();
        _database.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_Returns_FailResult_When_User_Is_Not_Found()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task UpsertAsync_And_GetByIdAsync_Persist_And_Retrieve_User()
    {
        var id = Guid.NewGuid();
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", id, "Bilbo Baggins");

        await _sut.UpsertAsync(user);
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.Email.Should().Be("bilbo@shire.me");
        result.Value.Name.Should().Be("Bilbo Baggins");
    }

    [Fact]
    public async Task UpsertAsync_Returns_The_User_On_Insert()
    {
        var user = new User(DateTimeOffset.UtcNow, "gandalf@middleearth.me", Guid.NewGuid(), "Gandalf the Grey");

        var result = await _sut.UpsertAsync(user);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Name.Should().Be(user.Name);
    }

    [Fact]
    public async Task UpsertAsync_Updates_Existing_User()
    {
        var id = Guid.NewGuid();
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", id, "Bilbo Baggins");
        var updated = new User(user.Created, user.Email, id, "Bilbo Gamgee");
        await _sut.UpsertAsync(user);

        await _sut.UpsertAsync(updated);
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Bilbo Gamgee");
    }

    [Fact]
    public async Task UpsertAsync_Preserves_Email_On_Update()
    {
        var id = Guid.NewGuid();
        var user = new User(DateTimeOffset.UtcNow, "frodo@shire.me", id, "Frodo Baggins");
        var updated = new User(user.Created, user.Email, id, "Frodo Gamgee");
        await _sut.UpsertAsync(user);

        await _sut.UpsertAsync(updated);
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("frodo@shire.me");
    }

    [Fact]
    public async Task UpsertAsync_Preserves_DateTimeOffset_With_Timezone_Offset()
    {
        var created = new DateTimeOffset(2025, 4, 17, 12, 30, 45, 123, TimeSpan.FromHours(2));
        var user = new User(created, "bilbo@shire.me", Guid.NewGuid(), "Bilbo Baggins");
        await _sut.UpsertAsync(user);

        var result = await _sut.GetByIdAsync(user.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Created.Should().Be(created);
    }
}
