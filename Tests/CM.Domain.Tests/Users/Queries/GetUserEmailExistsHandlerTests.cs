namespace CM.Domain.Tests.Users.Queries;

public sealed class GetUserEmailExistsHandlerTests
{
    private readonly IUserDataService _dataService = Substitute.For<IUserDataService>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new GetUserEmailExistsHandler(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public async Task HandleAsync_Returns_Error_When_DataService_Returns_Error()
    {
        var query = new GetUserEmailExists("bilbo@shire.me");
        _dataService.ExistsByEmailAsync(query.Email)
            .Returns(Task.FromResult<Result<bool>>(new FailResult("DB error")));
        var sut = new GetUserEmailExistsHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Be("DB error");
    }

    [Fact]
    public async Task HandleAsync_Returns_True_When_Email_Exists()
    {
        var query = new GetUserEmailExists("bilbo@shire.me");
        _dataService.ExistsByEmailAsync(query.Email)
            .Returns(Task.FromResult<Result<bool>>(true));
        var sut = new GetUserEmailExistsHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_Returns_False_When_Email_Does_Not_Exist()
    {
        var query = new GetUserEmailExists("nobody@nowhere.me");
        _dataService.ExistsByEmailAsync(query.Email)
            .Returns(Task.FromResult<Result<bool>>(false));
        var sut = new GetUserEmailExistsHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_Passes_Email_To_DataService()
    {
        var query = new GetUserEmailExists("bilbo@shire.me");
        _dataService.ExistsByEmailAsync(Arg.Any<string>())
            .Returns(Task.FromResult<Result<bool>>(false));
        var sut = new GetUserEmailExistsHandler(_dataService);

        await sut.HandleAsync(query);

        await _dataService.Received(1).ExistsByEmailAsync("bilbo@shire.me");
    }
}
