namespace CM.Domain.Tests.CheckLists.Queries;

public sealed class GetCheckListHandlerTests
{
    private readonly ICheckListDataService _dataService = Substitute.For<ICheckListDataService>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new GetCheckListHandler(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public async Task HandleAsync_Returns_Error_When_DataService_Returns_Error()
    {
        var query = new GetCheckList(Guid.NewGuid());
        _dataService.GetByIdAsync(query.CheckListId)
            .Returns(Task.FromResult<Result<CheckList>>(new FailResult("Not found")));
        var sut = new GetCheckListHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Be("Not found");
    }

    [Fact]
    public async Task HandleAsync_Returns_CheckListDto_When_CheckList_Exists()
    {
        var checkListId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var checkList = new CheckList(DateTimeOffset.UtcNow, checkListId, "My list", userId);
        var query = new GetCheckList(checkListId);
        _dataService.GetByIdAsync(query.CheckListId)
            .Returns(Task.FromResult<Result<CheckList>>(checkList));
        var sut = new GetCheckListHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(checkListId);
        result.Value.Name.Should().Be("My list");
        result.Value.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task HandleAsync_Maps_All_CheckList_Properties_To_Dto()
    {
        var checkListId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var created = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var checkList = new CheckList(created, checkListId, "Shopping", userId);
        var query = new GetCheckList(checkListId);
        _dataService.GetByIdAsync(query.CheckListId)
            .Returns(Task.FromResult<Result<CheckList>>(checkList));
        var sut = new GetCheckListHandler(_dataService);

        var result = await sut.HandleAsync(query);

        var dto = result.Value!;
        dto.Id.Should().Be(checkListId);
        dto.Name.Should().Be("Shopping");
        dto.UserId.Should().Be(userId);
        dto.Created.Should().Be(created);
    }
}
