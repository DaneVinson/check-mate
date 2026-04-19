namespace CM.Domain.Tests.Users.Commands;

public sealed class UpdateUserHandlerTests
{
    private readonly IUserDataService _dataService = Substitute.For<IUserDataService>();
    private readonly IMessenger<IEvent> _messenger = Substitute.For<IMessenger<IEvent>>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new UpdateUserHandler(null!, _messenger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_Messenger_Is_Null()
    {
        var act = () => new UpdateUserHandler(_dataService, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("messenger");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Sends_CommandFailed_When_Name_Is_Empty_Or_Whitespace(string name)
    {
        var command = new UpdateUser(name, Guid.NewGuid());
        var sut = new UpdateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Does_Not_Call_DataService_When_Name_Is_Empty_Or_Whitespace(string name)
    {
        var command = new UpdateUser(name, Guid.NewGuid());
        var sut = new UpdateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.DidNotReceive().GetByIdAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_GetByIdAsync_Returns_Error()
    {
        var command = new UpdateUser("Bilbo Baggins", Guid.NewGuid());
        _dataService.GetByIdAsync(command.UserId)
            .Returns(Task.FromResult<Result<User>>(new FailResult("Not found")));
        var sut = new UpdateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("Not found");
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_UpsertAsync_Returns_Error()
    {
        var userId = Guid.NewGuid();
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", userId, "Old name");
        var command = new UpdateUser("New name", userId);
        _dataService.GetByIdAsync(userId)
            .Returns(Task.FromResult<Result<User>>(user));
        _dataService.UpsertAsync(Arg.Any<User>())
            .Returns(Task.FromResult<Result<User>>(new FailResult("Upsert failed")));
        var sut = new UpdateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("Upsert failed");
    }

    [Fact]
    public async Task HandleAsync_Sets_Name_Before_Upserting()
    {
        var userId = Guid.NewGuid();
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", userId, "Old name");
        var command = new UpdateUser("New name", userId);
        _dataService.GetByIdAsync(userId)
            .Returns(Task.FromResult<Result<User>>(user));
        _dataService.UpsertAsync(Arg.Any<User>())
            .Returns(x => Task.FromResult<Result<User>>(x.Arg<User>()));
        var sut = new UpdateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.Received(1).UpsertAsync(Arg.Is<User>(u => u.Name == "New name"));
    }

    [Fact]
    public async Task HandleAsync_Sends_UserUpdated_When_Command_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", userId, "Old name");
        var command = new UpdateUser("New name", userId);
        _dataService.GetByIdAsync(userId)
            .Returns(Task.FromResult<Result<User>>(user));
        _dataService.UpsertAsync(Arg.Any<User>())
            .Returns(x => Task.FromResult<Result<User>>(x.Arg<User>()));
        var sut = new UpdateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<UserUpdated>().Which.CommandId.Should().Be(command.Id);
    }

    [Fact]
    public async Task HandleAsync_Does_Not_Upsert_When_GetByIdAsync_Returns_Error()
    {
        var command = new UpdateUser("New name", Guid.NewGuid());
        _dataService.GetByIdAsync(command.UserId)
            .Returns(Task.FromResult<Result<User>>(new FailResult("Not found")));
        var sut = new UpdateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.DidNotReceive().UpsertAsync(Arg.Any<User>());
    }
}
