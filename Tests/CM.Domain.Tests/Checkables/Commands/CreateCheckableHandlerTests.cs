namespace CM.Domain.Tests.Checkables.Commands;

public sealed class CreateCheckableHandlerTests
{
    private readonly ICheckableDataService _dataService = Substitute.For<ICheckableDataService>();
    private readonly IMessenger<IEvent> _messenger = Substitute.For<IMessenger<IEvent>>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new CreateCheckableHandler(null!, _messenger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_Messenger_Is_Null()
    {
        var act = () => new CreateCheckableHandler(_dataService, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("messenger");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Sends_CommandFailed_When_Description_Is_Empty_Or_Whitespace(string description)
    {
        var command = new CreateCheckable(Guid.NewGuid(), description, Guid.NewGuid());
        var sut = new CreateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Does_Not_Call_DataService_When_Description_Is_Empty_Or_Whitespace(string description)
    {
        var command = new CreateCheckable(Guid.NewGuid(), description, Guid.NewGuid());
        var sut = new CreateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.DidNotReceive().UpsertAsync(Arg.Any<Checkable>());
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_UpsertAsync_Returns_Error()
    {
        var command = new CreateCheckable(Guid.NewGuid(), "A task", Guid.NewGuid());
        _dataService.UpsertAsync(Arg.Any<Checkable>())
            .Returns(Task.FromResult<Result<Checkable>>(new FailResult("DB error")));
        var sut = new CreateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("DB error");
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckableCreated_When_Command_Is_Valid()
    {
        var checkListId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateCheckable(checkListId, "A task", userId);
        var checkable = new Checkable(false, checkListId, DateTimeOffset.UtcNow, "A task", Guid.NewGuid(), userId);
        _dataService.UpsertAsync(Arg.Any<Checkable>())
            .Returns(Task.FromResult<Result<Checkable>>(checkable));
        var sut = new CreateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CheckableCreated>().Which.CommandId.Should().Be(command.Id);
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckableCreated_With_Correct_Dto_When_Command_Is_Valid()
    {
        var checkListId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var command = new CreateCheckable(checkListId, "A task", userId);
        var checkable = new Checkable(false, checkListId, DateTimeOffset.UtcNow, "A task", id, userId);
        _dataService.UpsertAsync(Arg.Any<Checkable>())
            .Returns(Task.FromResult<Result<Checkable>>(checkable));
        var sut = new CreateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var @event = (CheckableCreated)_messenger.ReceivedCalls().Single().GetArguments()[0]!;
        @event.Checkable.Id.Should().Be(id);
        @event.Checkable.Description.Should().Be("A task");
    }
}
