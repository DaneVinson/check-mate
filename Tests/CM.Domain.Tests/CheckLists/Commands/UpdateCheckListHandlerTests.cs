namespace CM.Domain.Tests.CheckLists.Commands;

public sealed class UpdateCheckListHandlerTests
{
    private readonly ICheckListDataService _dataService = Substitute.For<ICheckListDataService>();
    private readonly IMessenger<IEvent> _messenger = Substitute.For<IMessenger<IEvent>>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new UpdateCheckListHandler(null!, _messenger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_Messenger_Is_Null()
    {
        var act = () => new UpdateCheckListHandler(_dataService, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("messenger");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Sends_CommandFailed_When_Name_Is_Empty_Or_Whitespace(string name)
    {
        var command = new UpdateCheckList(Guid.NewGuid(), name);
        var sut = new UpdateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Does_Not_Call_DataService_When_Name_Is_Empty_Or_Whitespace(string name)
    {
        var command = new UpdateCheckList(Guid.NewGuid(), name);
        var sut = new UpdateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.DidNotReceive().GetByIdAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_GetByIdAsync_Returns_Error()
    {
        var command = new UpdateCheckList(Guid.NewGuid(), "New name");
        _dataService.GetByIdAsync(command.CheckListId)
            .Returns(Task.FromResult<Result<CheckList>>(new FailResult("Not found")));
        var sut = new UpdateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("Not found");
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_UpsertAsync_Returns_Error()
    {
        var checkListId = Guid.NewGuid();
        var checkList = new CheckList(DateTimeOffset.UtcNow, checkListId, "Old name", Guid.NewGuid());
        var command = new UpdateCheckList(checkListId, "New name");
        _dataService.GetByIdAsync(checkListId)
            .Returns(Task.FromResult<Result<CheckList>>(checkList));
        _dataService.UpsertAsync(Arg.Any<CheckList>())
            .Returns(Task.FromResult<Result<CheckList>>(new FailResult("Upsert failed")));
        var sut = new UpdateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("Upsert failed");
    }

    [Fact]
    public async Task HandleAsync_Sets_Name_Before_Upserting()
    {
        var checkListId = Guid.NewGuid();
        var checkList = new CheckList(DateTimeOffset.UtcNow, checkListId, "Old name", Guid.NewGuid());
        var command = new UpdateCheckList(checkListId, "New name");
        _dataService.GetByIdAsync(checkListId)
            .Returns(Task.FromResult<Result<CheckList>>(checkList));
        _dataService.UpsertAsync(Arg.Any<CheckList>())
            .Returns(x => Task.FromResult<Result<CheckList>>(x.Arg<CheckList>()));
        var sut = new UpdateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.Received(1).UpsertAsync(Arg.Is<CheckList>(cl => cl.Name == "New name"));
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckListUpdated_When_Command_Is_Valid()
    {
        var checkListId = Guid.NewGuid();
        var checkList = new CheckList(DateTimeOffset.UtcNow, checkListId, "Old name", Guid.NewGuid());
        var command = new UpdateCheckList(checkListId, "New name");
        _dataService.GetByIdAsync(checkListId)
            .Returns(Task.FromResult<Result<CheckList>>(checkList));
        _dataService.UpsertAsync(Arg.Any<CheckList>())
            .Returns(x => Task.FromResult<Result<CheckList>>(x.Arg<CheckList>()));
        var sut = new UpdateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CheckListUpdated>().Which.CommandId.Should().Be(command.Id);
    }
}
