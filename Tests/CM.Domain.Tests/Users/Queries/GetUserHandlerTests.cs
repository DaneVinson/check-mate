namespace CM.Domain.Tests.Users.Queries;

public sealed class GetUserHandlerTests
{
    private readonly IUserDataService _dataService = Substitute.For<IUserDataService>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new GetUserHandler(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public async Task HandleAsync_Returns_Error_When_DataService_Returns_Error()
    {
        var query = new GetUser(Guid.NewGuid());
        _dataService.GetByIdAsync(query.UserId)
            .Returns(Task.FromResult<Result<User>>(new FailResult("Not found")));
        var sut = new GetUserHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Be("Not found");
    }

    [Fact]
    public async Task HandleAsync_Returns_UserDto_When_User_Exists()
    {
        var userId = Guid.NewGuid();
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", userId, "Bilbo Baggins");
        var query = new GetUser(userId);
        _dataService.GetByIdAsync(query.UserId)
            .Returns(Task.FromResult<Result<User>>(user));
        var sut = new GetUserHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(userId);
        result.Value.Email.Should().Be("bilbo@shire.me");
        result.Value.Name.Should().Be("Bilbo Baggins");
    }

    [Fact]
    public async Task HandleAsync_Maps_All_User_Properties_To_Dto()
    {
        var userId = Guid.NewGuid();
        var created = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var user = new User(created, "bilbo@shire.me", userId, "Bilbo Baggins");
        var query = new GetUser(userId);
        _dataService.GetByIdAsync(query.UserId)
            .Returns(Task.FromResult<Result<User>>(user));
        var sut = new GetUserHandler(_dataService);

        var result = await sut.HandleAsync(query);

        var dto = result.Value!;
        dto.Id.Should().Be(userId);
        dto.Email.Should().Be("bilbo@shire.me");
        dto.Name.Should().Be("Bilbo Baggins");
        dto.Created.Should().Be(created);
    }
}
