namespace CM.Domain.Tests.Users.Commands;

public sealed class CreateUserHandlerTests
{
    private readonly IUserDataService _dataService = Substitute.For<IUserDataService>();
    private readonly IMessenger<IEvent> _messenger = Substitute.For<IMessenger<IEvent>>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new CreateUserHandler(null!, _messenger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_Messenger_Is_Null()
    {
        var act = () => new CreateUserHandler(_dataService, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("messenger");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Sends_CommandFailed_When_Email_Is_Empty_Or_Whitespace(string email)
    {
        var command = new CreateUser(email, "Bilbo Baggins");
        var sut = new CreateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing-at-sign")]
    [InlineData("@nodomain")]
    [InlineData("noatsign.com")]
    public async Task HandleAsync_Sends_CommandFailed_When_Email_Is_Invalid_Format(string email)
    {
        var command = new CreateUser(email, "Bilbo Baggins");
        var sut = new CreateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing-at-sign")]
    [InlineData("@nodomain")]
    [InlineData("noatsign.com")]
    public async Task HandleAsync_Does_Not_Call_DataService_When_Email_Is_Invalid_Format(string email)
    {
        var command = new CreateUser(email, "Bilbo Baggins");
        var sut = new CreateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.DidNotReceive().UpsertAsync(Arg.Any<User>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Sends_CommandFailed_When_Name_Is_Empty_Or_Whitespace(string name)
    {
        var command = new CreateUser("bilbo@shire.me", name);
        var sut = new CreateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Theory]
    [InlineData("", "Bilbo Baggins")]
    [InlineData("   ", "Bilbo Baggins")]
    [InlineData("not-an-email", "Bilbo Baggins")]
    [InlineData("bilbo@shire.me", "")]
    [InlineData("bilbo@shire.me", "   ")]
    public async Task HandleAsync_Does_Not_Call_DataService_When_Validation_Fails(string email, string name)
    {
        var command = new CreateUser(email, name);
        var sut = new CreateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.DidNotReceive().UpsertAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_UpsertAsync_Returns_Error()
    {
        var command = new CreateUser("bilbo@shire.me", "Bilbo Baggins");
        _dataService.UpsertAsync(Arg.Any<User>())
            .Returns(Task.FromResult<Result<User>>(new FailResult("DB error")));
        var sut = new CreateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("DB error");
    }

    [Fact]
    public async Task HandleAsync_Sends_UserCreated_When_Command_Is_Valid()
    {
        var command = new CreateUser("bilbo@shire.me", "Bilbo Baggins");
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", Guid.NewGuid(), "Bilbo Baggins");
        _dataService.UpsertAsync(Arg.Any<User>())
            .Returns(Task.FromResult<Result<User>>(user));
        var sut = new CreateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<UserCreated>().Which.CommandId.Should().Be(command.Id);
    }

    [Fact]
    public async Task HandleAsync_Sends_UserCreated_With_Correct_Dto_When_Command_Is_Valid()
    {
        var id = Guid.NewGuid();
        var command = new CreateUser("bilbo@shire.me", "Bilbo Baggins");
        var user = new User(DateTimeOffset.UtcNow, "bilbo@shire.me", id, "Bilbo Baggins");
        _dataService.UpsertAsync(Arg.Any<User>())
            .Returns(Task.FromResult<Result<User>>(user));
        var sut = new CreateUserHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var @event = (UserCreated)_messenger.ReceivedCalls().Single().GetArguments()[0]!;
        @event.User.Id.Should().Be(id);
        @event.User.Email.Should().Be("bilbo@shire.me");
        @event.User.Name.Should().Be("Bilbo Baggins");
    }
}
