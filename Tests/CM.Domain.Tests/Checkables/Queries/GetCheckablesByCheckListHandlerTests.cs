namespace CM.Domain.Tests.Checkables.Queries;

public sealed class GetCheckablesByCheckListHandlerTests
{
    private readonly ICheckableDataService _dataService = Substitute.For<ICheckableDataService>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new GetCheckablesByCheckListHandler(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public async Task HandleAsync_Returns_Error_When_DataService_Returns_Error()
    {
        var query = new GetCheckablesByCheckList(Guid.NewGuid());
        _dataService.GetByCheckListAsync(query.CheckListId)
            .Returns(Task.FromResult(Result<IReadOnlyList<Checkable>>.Fail(new FailResult("DB error"))));
        var sut = new GetCheckablesByCheckListHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Be("DB error");
    }

    [Fact]
    public async Task HandleAsync_Returns_Empty_List_When_No_Checkables_Exist()
    {
        var query = new GetCheckablesByCheckList(Guid.NewGuid());
        _dataService.GetByCheckListAsync(query.CheckListId)
            .Returns(Task.FromResult(Result<IReadOnlyList<Checkable>>.Success(new List<Checkable>())));
        var sut = new GetCheckablesByCheckListHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_Returns_Mapped_Dtos_When_Checkables_Exist()
    {
        var checkListId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var checkable = new Checkable(true, checkListId, DateTimeOffset.UtcNow, "Task", id, userId);
        var query = new GetCheckablesByCheckList(checkListId);
        _dataService.GetByCheckListAsync(query.CheckListId)
            .Returns(Task.FromResult(Result<IReadOnlyList<Checkable>>.Success(new List<Checkable> { checkable })));
        var sut = new GetCheckablesByCheckListHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle().Which.Id.Should().Be(id);
    }

    [Fact]
    public async Task HandleAsync_Maps_All_Checkable_Properties_To_Dto()
    {
        var checkListId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var created = DateTimeOffset.UtcNow;
        var checkable = new Checkable(true, checkListId, created, "My task", id, userId);
        var query = new GetCheckablesByCheckList(checkListId);
        _dataService.GetByCheckListAsync(query.CheckListId)
            .Returns(Task.FromResult(Result<IReadOnlyList<Checkable>>.Success(new List<Checkable> { checkable })));
        var sut = new GetCheckablesByCheckListHandler(_dataService);

        var result = await sut.HandleAsync(query);

        var dto = result.Value.Single();
        dto.Id.Should().Be(id);
        dto.Checked.Should().BeTrue();
        dto.CheckListId.Should().Be(checkListId);
        dto.Description.Should().Be("My task");
        dto.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task HandleAsync_Returns_Multiple_Dtos_When_Multiple_Checkables_Exist()
    {
        var checkListId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var checkables = new List<Checkable>
        {
            new(false, checkListId, DateTimeOffset.UtcNow, "Task 1", Guid.NewGuid(), userId),
            new(true, checkListId, DateTimeOffset.UtcNow, "Task 2", Guid.NewGuid(), userId),
            new(false, checkListId, DateTimeOffset.UtcNow, "Task 3", Guid.NewGuid(), userId),
        };
        var query = new GetCheckablesByCheckList(checkListId);
        _dataService.GetByCheckListAsync(query.CheckListId)
            .Returns(Task.FromResult(Result<IReadOnlyList<Checkable>>.Success(checkables)));
        var sut = new GetCheckablesByCheckListHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }
}
