namespace CM.Domain.Tests.CheckLists.Queries;

public sealed class GetCheckListsByUserHandlerTests
{
    private readonly ICheckListDataService _dataService = Substitute.For<ICheckListDataService>();

    [Fact]
    public void Constructor_Throws_ArgumentNullException_When_DataService_Is_Null()
    {
        var act = () => new GetCheckListsByUserHandler(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("dataService");
    }

    [Fact]
    public async Task HandleAsync_Returns_Error_When_DataService_Returns_Error()
    {
        var query = new GetCheckListsByUser(Guid.NewGuid());
        _dataService.GetByUserAsync(query.UserId)
            .Returns(Task.FromResult(Result<IReadOnlyList<CheckList>>.Fail(new FailResult("DB error"))));
        var sut = new GetCheckListsByUserHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Be("DB error");
    }

    [Fact]
    public async Task HandleAsync_Returns_Empty_List_When_No_CheckLists_Exist()
    {
        var query = new GetCheckListsByUser(Guid.NewGuid());
        _dataService.GetByUserAsync(query.UserId)
            .Returns(Task.FromResult(Result<IReadOnlyList<CheckList>>.Success(new List<CheckList>())));
        var sut = new GetCheckListsByUserHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_Returns_Mapped_Dtos_When_CheckLists_Exist()
    {
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var checkList = new CheckList(DateTimeOffset.UtcNow, id, "My list", userId);
        var query = new GetCheckListsByUser(userId);
        _dataService.GetByUserAsync(query.UserId)
            .Returns(Task.FromResult(Result<IReadOnlyList<CheckList>>.Success(new List<CheckList> { checkList })));
        var sut = new GetCheckListsByUserHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle().Which.Id.Should().Be(id);
    }

    [Fact]
    public async Task HandleAsync_Maps_All_CheckList_Properties_To_Dto()
    {
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var created = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var checkList = new CheckList(created, id, "Groceries", userId);
        var query = new GetCheckListsByUser(userId);
        _dataService.GetByUserAsync(query.UserId)
            .Returns(Task.FromResult(Result<IReadOnlyList<CheckList>>.Success(new List<CheckList> { checkList })));
        var sut = new GetCheckListsByUserHandler(_dataService);

        var result = await sut.HandleAsync(query);

        var dto = result.Value.Single();
        dto.Id.Should().Be(id);
        dto.Name.Should().Be("Groceries");
        dto.UserId.Should().Be(userId);
        dto.Created.Should().Be(created);
    }

    [Fact]
    public async Task HandleAsync_Returns_Multiple_Dtos_When_Multiple_CheckLists_Exist()
    {
        var userId = Guid.NewGuid();
        var checkLists = new List<CheckList>
        {
            new(DateTimeOffset.UtcNow, Guid.NewGuid(), "List 1", userId),
            new(DateTimeOffset.UtcNow, Guid.NewGuid(), "List 2", userId),
        };
        var query = new GetCheckListsByUser(userId);
        _dataService.GetByUserAsync(query.UserId)
            .Returns(Task.FromResult(Result<IReadOnlyList<CheckList>>.Success(checkLists)));
        var sut = new GetCheckListsByUserHandler(_dataService);

        var result = await sut.HandleAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}
