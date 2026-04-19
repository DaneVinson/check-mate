namespace CM.Domain.Tests.CheckLists.Commands;

public sealed class CreateCheckListHandlerTests
{
    private readonly ICheckListDataService _dataService = Substitute.For<ICheckListDataService>();
    private readonly IMessenger<IEvent> _messenger = Substitute.For<IMessenger<IEvent>>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new CreateCheckListHandler(null!, _messenger);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_Messenger_Is_Null()
    {
        var act = () => new CreateCheckListHandler(_dataService, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("messenger");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Sends_CommandFailed_When_Name_Is_Empty_Or_Whitespace(string name)
    {
        var command = new CreateCheckList(name, Guid.NewGuid());
        var sut = new CreateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.CommandId.Should().Be(command.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_Does_Not_Call_DataService_When_Name_Is_Empty_Or_Whitespace(string name)
    {
        var command = new CreateCheckList(name, Guid.NewGuid());
        var sut = new CreateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        await _dataService.DidNotReceive().UpsertAsync(Arg.Any<CheckList>());
    }

    [Fact]
    public async Task HandleAsync_Sends_CommandFailed_When_UpsertAsync_Returns_Error()
    {
        var command = new CreateCheckList("My list", Guid.NewGuid());
        _dataService.UpsertAsync(Arg.Any<CheckList>())
            .Returns(Task.FromResult<Result<CheckList>>(new FailResult("DB error")));
        var sut = new CreateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CommandFailed>().Which.Message.Should().Be("DB error");
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckListCreated_When_Command_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var command = new CreateCheckList("My list", userId);
        var checkList = new CheckList(DateTimeOffset.UtcNow, Guid.NewGuid(), "My list", userId);
        _dataService.UpsertAsync(Arg.Any<CheckList>())
            .Returns(Task.FromResult<Result<CheckList>>(checkList));
        var sut = new CreateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var sentEvent = _messenger.ReceivedCalls().Should().ContainSingle().Subject.GetArguments()[0];
        sentEvent.Should().BeOfType<CheckListCreated>().Which.CommandId.Should().Be(command.Id);
    }

    [Fact]
    public async Task HandleAsync_Sends_CheckListCreated_With_Correct_Dto_When_Command_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var command = new CreateCheckList("My list", userId);
        var checkList = new CheckList(DateTimeOffset.UtcNow, id, "My list", userId);
        _dataService.UpsertAsync(Arg.Any<CheckList>())
            .Returns(Task.FromResult<Result<CheckList>>(checkList));
        var sut = new CreateCheckListHandler(_dataService, _messenger);

        await sut.HandleAsync(command);

        var @event = (CheckListCreated)_messenger.ReceivedCalls().Single().GetArguments()[0]!;
        @event.CheckList.Id.Should().Be(id);
        @event.CheckList.Name.Should().Be("My list");
        @event.CheckList.UserId.Should().Be(userId);
    }
}
