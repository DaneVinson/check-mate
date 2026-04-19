namespace CM.Bogus.Tests.Users;

public sealed class UserDataServiceTests
{
    private static IUserDataService CreateService()
    {
        var services = new ServiceCollection();
        services.AddBogusDataServices();
        var provider = services.BuildServiceProvider();
        return provider.CreateScope().ServiceProvider.GetRequiredService<IUserDataService>();
    }

    [Fact]
    public async Task GetByIdAsync_Returns_User_When_Id_Exists()
    {
        var seededUser = DataSeed.GetUsers().First();
        var sut = CreateService();

        var result = await sut.GetByIdAsync(seededUser.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(seededUser.Id);
        result.Value.Email.Should().Be(seededUser.Email);
        result.Value.Name.Should().Be(seededUser.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_FailResult_When_Id_Does_Not_Exist()
    {
        var sut = CreateService();

        var result = await sut.GetByIdAsync(Guid.NewGuid());

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task UpsertAsync_Returns_User_When_Inserted()
    {
        var user = new User(DateTimeOffset.UtcNow, "frodo@shire.me", Guid.NewGuid(), "Frodo Baggins");
        var sut = CreateService();

        var result = await sut.UpsertAsync(user);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Name.Should().Be(user.Name);
    }

    [Fact]
    public async Task UpsertAsync_Updates_Existing_User()
    {
        var userId = Guid.NewGuid();
        var original = new User(DateTimeOffset.UtcNow, "gandalf@middleearth.me", userId, "Gandalf the Grey");
        var updated = new User(original.Created, original.Email, userId, "Gandalf the White");
        var sut = CreateService();
        await sut.UpsertAsync(original);

        await sut.UpsertAsync(updated);
        var result = await sut.GetByIdAsync(userId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Gandalf the White");
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Bilbo_Baggins_For_Fixed_Id()
    {
        var bilboId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var sut = CreateService();

        var result = await sut.GetByIdAsync(bilboId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Bilbo Baggins");
        result.Value.Email.Should().Be("bilbo.baggins@shire.me");
    }
}
