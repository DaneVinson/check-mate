namespace CM.Bogus.Tests.Users;

public sealed class UserDataServiceExistsByEmailTests
{
    private static IUserDataService CreateService()
    {
        var services = new ServiceCollection();
        services.AddBogusDataServices();
        var provider = services.BuildServiceProvider();
        return provider.CreateScope().ServiceProvider.GetRequiredService<IUserDataService>();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Returns_True_When_Email_Belongs_To_Seeded_User()
    {
        var sut = CreateService();

        var result = await sut.ExistsByEmailAsync("bilbo.baggins@shire.me");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Returns_False_When_Email_Does_Not_Exist()
    {
        var sut = CreateService();

        var result = await sut.ExistsByEmailAsync("nobody@nowhere.me");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Returns_True_After_User_With_That_Email_Is_Upserted()
    {
        var email = $"test-{Guid.NewGuid()}@example.com";
        var user = new User(DateTimeOffset.UtcNow, email, Guid.NewGuid(), "Test User");
        var sut = CreateService();
        await sut.UpsertAsync(user);

        var result = await sut.ExistsByEmailAsync(email);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Is_Case_Insensitive()
    {
        var sut = CreateService();

        var result = await sut.ExistsByEmailAsync("BILBO.BAGGINS@SHIRE.ME");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }
}
