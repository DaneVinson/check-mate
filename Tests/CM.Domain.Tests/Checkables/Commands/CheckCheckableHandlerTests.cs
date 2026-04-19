namespace CM.Domain.Tests.Checkables.Commands;

public sealed class CheckCheckableHandlerTests
{
    private readonly ICheckableDataService _dataService = Substitute.For<ICheckableDataService>();
    private readonly IMessenger<IEvent> _messenger = Substitute.For<IMessenger<IEvent>>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new CheckCheckableHandler(null!, _messenger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_Messenger_Is_Null()
    {
        var act = () => new CheckCheckableHandler(_dataService, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("messenger");
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_GetByIdAsync_Returns_Error()
    {
        var command = new CheckCheckable(Guid.NewGuid());
        _dataService.GetByIdAsync(command.CheckableId)
            .Returns(Task.FromResult<Result<Checkable>>(new FailResult("Not found")));
        var sut = new CheckCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_UpsertAsync_Returns_Error()
    {
        var checkableId = Guid.NewGuid();
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Task", checkableId, Guid.NewGuid());
        var command = new CheckCheckable(checkableId);
        _dataService.GetByIdAsync(checkableId)
            .Returns(Task.FromResult<Result<Checkable>>(checkable));
        _dataService.UpsertAsync(Arg.Any<Checkable>())
            .Returns(Task.FromResult<Result<Checkable>>(new FailResult("Upsert failed")));
        var sut = new CheckCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("Upsert failed");
    }

    [Fact]
    public async Task HandleAsync_Sets_Checked_To_True_Before_Upserting()
    {
        var checkableId = Guid.NewGuid();
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Task", checkableId, Guid.NewGuid());
        var command = new CheckCheckable(checkableId);
        _dataService.GetByIdAsync(checkableId)
            .Returns(Task.FromResult<Result<Checkable>>(checkable));
        _dataService.UpsertAsync(Arg.Any<Checkable>())
            .Returns(x => Task.FromResult<Result<Checkable>>(x.Arg<Checkable>()));
        var sut = new CheckCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.Received(1).UpsertAsync(Arg.Is<Checkable>(c => c.Checked));
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckableChecked_When_Command_Succeeds()
    {
        var checkableId = Guid.NewGuid();
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Task", checkableId, Guid.NewGuid());
        var command = new CheckCheckable(checkableId);
        _dataService.GetByIdAsync(checkableId)
            .Returns(Task.FromResult<Result<Checkable>>(checkable));
        _dataService.UpsertAsync(Arg.Any<Checkable>())
            .Returns(x => Task.FromResult<Result<Checkable>>(x.Arg<Checkable>()));
        var sut = new CheckCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CheckableChecked>().Which.CommandId.Should().Be(command.Id);
    }

    [Fact]
    public async Task HandleAsync_Does_Not_Upsert_When_GetByIdAsync_Returns_Error()
    {
        var command = new CheckCheckable(Guid.NewGuid());
        _dataService.GetByIdAsync(command.CheckableId)
            .Returns(Task.FromResult<Result<Checkable>>(new FailResult("Not found")));
        var sut = new CheckCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.DidNotReceive().UpsertAsync(Arg.Any<Checkable>());
    }
}
