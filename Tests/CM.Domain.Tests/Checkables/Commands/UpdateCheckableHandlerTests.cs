namespace CM.Domain.Tests.Checkables.Commands;

public sealed class UpdateCheckableHandlerTests
{
    private readonly ICheckableDataService _dataService = Substitute.For<ICheckableDataService>();
    private readonly IMessenger<IEvent> _messenger = Substitute.For<IMessenger<IEvent>>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new UpdateCheckableHandler(null!, _messenger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_Messenger_Is_Null()
    {
        var act = () => new UpdateCheckableHandler(_dataService, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("messenger");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Sends_CommandFailed_When_Description_Is_Empty_Or_Whitespace(string description)
    {
        var command = new UpdateCheckable(Guid.NewGuid(), description);
        var sut = new UpdateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Does_Not_Call_DataService_When_Description_Is_Empty_Or_Whitespace(string description)
    {
        var command = new UpdateCheckable(Guid.NewGuid(), description);
        var sut = new UpdateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.DidNotReceive().GetByIdAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_GetByIdAsync_Returns_Error()
    {
        var command = new UpdateCheckable(Guid.NewGuid(), "New description");
        _dataService.GetByIdAsync(command.CheckableId)
            .Returns(Task.FromResult<Result<Checkable>>(new FailResult("Not found")));
        var sut = new UpdateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("Not found");
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_UpsertAsync_Returns_Error()
    {
        var checkableId = Guid.NewGuid();
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Old description", checkableId, Guid.NewGuid());
        var command = new UpdateCheckable(checkableId, "New description");
        _dataService.GetByIdAsync(checkableId)
            .Returns(Task.FromResult<Result<Checkable>>(checkable));
        _dataService.UpsertAsync(Arg.Any<Checkable>())
            .Returns(Task.FromResult<Result<Checkable>>(new FailResult("Upsert failed")));
        var sut = new UpdateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("Upsert failed");
    }

    [Fact]
    public async Task HandleAsync_Sets_Description_Before_Upserting()
    {
        var checkableId = Guid.NewGuid();
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Old description", checkableId, Guid.NewGuid());
        var command = new UpdateCheckable(checkableId, "New description");
        _dataService.GetByIdAsync(checkableId)
            .Returns(Task.FromResult<Result<Checkable>>(checkable));
        _dataService.UpsertAsync(Arg.Any<Checkable>())
            .Returns(x => Task.FromResult<Result<Checkable>>(x.Arg<Checkable>()));
        var sut = new UpdateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.Received(1).UpsertAsync(Arg.Is<Checkable>(c => c.Description == "New description"));
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckableUpdated_When_Command_Is_Valid()
    {
        var checkableId = Guid.NewGuid();
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Old description", checkableId, Guid.NewGuid());
        var command = new UpdateCheckable(checkableId, "New description");
        _dataService.GetByIdAsync(checkableId)
            .Returns(Task.FromResult<Result<Checkable>>(checkable));
        _dataService.UpsertAsync(Arg.Any<Checkable>())
            .Returns(x => Task.FromResult<Result<Checkable>>(x.Arg<Checkable>()));
        var sut = new UpdateCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CheckableUpdated>().Which.CommandId.Should().Be(command.Id);
    }
}
