namespace CM.Domain.Tests.CheckLists.Commands;

public sealed class DeleteCheckListHandlerTests
{
    private readonly ICheckListDataService _dataService = Substitute.For<ICheckListDataService>();
    private readonly IMessenger<IEvent> _messenger = Substitute.For<IMessenger<IEvent>>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new DeleteCheckListHandler(null!, _messenger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_Messenger_Is_Null()
    {
        var act = () => new DeleteCheckListHandler(_dataService, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("messenger");
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_DeleteAsync_Returns_Error()
    {
        var command = new DeleteCheckList(Guid.NewGuid());
        _dataService.DeleteAsync(command.CheckListId)
            .Returns(Task.FromResult<Result<bool>>(new FailResult("Not found")));
        var sut = new DeleteCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_With_DataService_Message_On_Error()
    {
        var command = new DeleteCheckList(Guid.NewGuid());
        _dataService.DeleteAsync(command.CheckListId)
            .Returns(Task.FromResult<Result<bool>>(new FailResult("CheckList not found.")));
        var sut = new DeleteCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = (CommandFailed)_messenger.ReceivedCalls().Single().GetArguments()[0]!;
        sentEvent.Message.Should().Be("CheckList not found.");
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckListDeleted_When_Delete_Succeeds()
    {
        var command = new DeleteCheckList(Guid.NewGuid());
        _dataService.DeleteAsync(command.CheckListId)
            .Returns(Task.FromResult<Result<bool>>(true));
        var sut = new DeleteCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CheckListDeleted>().Which.CheckListId.Should().Be(command.CheckListId);
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckListDeleted_With_Correct_CommandId()
    {
        var command = new DeleteCheckList(Guid.NewGuid());
        _dataService.DeleteAsync(command.CheckListId)
            .Returns(Task.FromResult<Result<bool>>(true));
        var sut = new DeleteCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var @event = (CheckListDeleted)_messenger.ReceivedCalls().Single().GetArguments()[0]!;
        @event.CommandId.Should().Be(command.Id);
    }
}
