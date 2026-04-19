namespace CM.Domain.Tests.Checkables.Commands;

public sealed class DeleteCheckableHandlerTests
{
    private readonly ICheckableDataService _dataService = Substitute.For<ICheckableDataService>();
    private readonly IMessenger<IEvent> _messenger = Substitute.For<IMessenger<IEvent>>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new DeleteCheckableHandler(null!, _messenger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_Messenger_Is_Null()
    {
        var act = () => new DeleteCheckableHandler(_dataService, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("messenger");
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_DeleteAsync_Returns_Error()
    {
        var command = new DeleteCheckable(Guid.NewGuid());
        _dataService.DeleteAsync(command.CheckableId)
            .Returns(Task.FromResult<Result<bool>>(new FailResult("Not found")));
        var sut = new DeleteCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_With_DataService_Message_On_Error()
    {
        var command = new DeleteCheckable(Guid.NewGuid());
        _dataService.DeleteAsync(command.CheckableId)
            .Returns(Task.FromResult<Result<bool>>(new FailResult("Checkable not found.")));
        var sut = new DeleteCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = (CommandFailed)_messenger.ReceivedCalls().Single().GetArguments()[0]!;
        sentEvent.Message.Should().Be("Checkable not found.");
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckableDeleted_When_Delete_Succeeds()
    {
        var command = new DeleteCheckable(Guid.NewGuid());
        _dataService.DeleteAsync(command.CheckableId)
            .Returns(Task.FromResult<Result<bool>>(true));
        var sut = new DeleteCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CheckableDeleted>().Which.CheckableId.Should().Be(command.CheckableId);
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckableDeleted_With_Correct_CommandId()
    {
        var command = new DeleteCheckable(Guid.NewGuid());
        _dataService.DeleteAsync(command.CheckableId)
            .Returns(Task.FromResult<Result<bool>>(true));
        var sut = new DeleteCheckableHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var @event = (CheckableDeleted)_messenger.ReceivedCalls().Single().GetArguments()[0]!;
        @event.CommandId.Should().Be(command.Id);
    }
}
