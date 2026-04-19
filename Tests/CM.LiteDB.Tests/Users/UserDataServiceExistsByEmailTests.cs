namespace CM.LiteDB.Tests.Users;

public sealed class UserDataServiceExistsByEmailTests : IDisposable
{
    private readonly LiteDatabase _database = new(":memory:");
    private readonly ServiceProvider _provider;
    private readonly IUserDataService _sut;

    public UserDataServiceExistsByEmailTests()
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
    public async Task ExistsByEmailAsync_Returns_False_When_No_Users_Exist()
    {
        var result = await _sut.ExistsByEmailAsync("bilbo@shire.me");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Returns_True_After_User_With_That_Email_Is_Upserted()
    {
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", Guid.NewGuid(), "Bilbo Baggins");
        await _sut.UpsertAsync(user);

        var result = await _sut.ExistsByEmailAsync("bilbo@shire.me");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Returns_False_When_Email_Does_Not_Match()
    {
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", Guid.NewGuid(), "Bilbo Baggins");
        await _sut.UpsertAsync(user);

        var result = await _sut.ExistsByEmailAsync("frodo@shire.me");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Is_Case_Insensitive()
    {
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", Guid.NewGuid(), "Bilbo Baggins");
        await _sut.UpsertAsync(user);

        var result = await _sut.ExistsByEmailAsync("BILBO@SHIRE.ME");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Returns_True_When_One_Of_Multiple_Users_Matches()
    {
        await _sut.UpsertAsync(new User(DateTimeOffset.UtcNow, "bilbo@shire.me", Guid.NewGuid(), "Bilbo Baggins"));
        await _sut.UpsertAsync(new User(DateTimeOffset.UtcNow, "frodo@shire.me", Guid.NewGuid(), "Frodo Baggins"));

        var result = await _sut.ExistsByEmailAsync("frodo@shire.me");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }
}
