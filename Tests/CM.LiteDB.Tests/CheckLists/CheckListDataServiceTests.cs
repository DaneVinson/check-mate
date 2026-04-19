namespace CM.LiteDB.Tests.CheckLists;

public sealed class CheckListDataServiceTests : IDisposable
{
    private readonly LiteDatabase _database = new(":memory:");
    private readonly ServiceProvider _provider;
    private readonly ICheckListDataService _sut;

    public CheckListDataServiceTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILiteDatabase>(_database);
        services.AddLiteDbDataServices();
        _provider = services.BuildServiceProvider();
        _sut = _provider.CreateScope().ServiceProvider.GetRequiredService<ICheckListDataService>();
    }

    public void Dispose()
    {
        _provider.Dispose();
        _database.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_Returns_FailResult_When_CheckList_Is_Not_Found()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task UpsertAsync_And_GetByIdAsync_Persist_And_Retrieve_CheckList()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var checkList = new CheckList(DateTimeOffset.UtcNow, id, "My list", userId);

        await _sut.UpsertAsync(checkList);
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.Name.Should().Be("My list");
        result.Value.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task UpsertAsync_Returns_The_CheckList_On_Insert()
    {
        var checkList = new CheckList(DateTimeOffset.UtcNow, Guid.NewGuid(), "Test list", Guid.NewGuid());

        var result = await _sut.UpsertAsync(checkList);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(checkList.Id);
        result.Value.Name.Should().Be(checkList.Name);
    }

    [Fact]
    public async Task UpsertAsync_Updates_Existing_CheckList()
    {
        var id = Guid.NewGuid();
        var checkList = new CheckList(DateTimeOffset.UtcNow, id, "Original", Guid.NewGuid());
        var updated = new CheckList(checkList.Created, id, "Updated", checkList.UserId);
        await _sut.UpsertAsync(checkList);

        await _sut.UpsertAsync(updated);
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task GetByUserAsync_Returns_CheckLists_For_User()
    {
        var userId = Guid.NewGuid();
        var list1 = new CheckList(DateTimeOffset.UtcNow, Guid.NewGuid(), "List 1", userId);
        var list2 = new CheckList(DateTimeOffset.UtcNow, Guid.NewGuid(), "List 2", userId);
        var otherList = new CheckList(DateTimeOffset.UtcNow, Guid.NewGuid(), "Other list", Guid.NewGuid());
        await _sut.UpsertAsync(list1);
        await _sut.UpsertAsync(list2);
        await _sut.UpsertAsync(otherList);

        var result = await _sut.GetByUserAsync(userId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(cl => cl.UserId.Should().Be(userId));
    }

    [Fact]
    public async Task GetByUserAsync_Returns_Empty_List_When_No_CheckLists_Exist_For_User()
    {
        var result = await _sut.GetByUserAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_When_CheckList_Is_Deleted()
    {
        var checkList = new CheckList(DateTimeOffset.UtcNow, Guid.NewGuid(), "To delete", Guid.NewGuid());
        await _sut.UpsertAsync(checkList);

        var result = await _sut.DeleteAsync(checkList.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Returns_FailResult_When_CheckList_Does_Not_Exist()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task DeleteAsync_Removes_CheckList_So_It_Cannot_Be_Retrieved()
    {
        var checkList = new CheckList(DateTimeOffset.UtcNow, Guid.NewGuid(), "To delete", Guid.NewGuid());
        await _sut.UpsertAsync(checkList);
        await _sut.DeleteAsync(checkList.Id);

        var result = await _sut.GetByIdAsync(checkList.Id);

        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task UpsertAsync_Preserves_DateTimeOffset_With_Timezone_Offset()
    {
        var created = new DateTimeOffset(2025, 4, 17, 12, 30, 45, 123, TimeSpan.FromHours(2));
        var checkList = new CheckList(created, Guid.NewGuid(), "Time test list", Guid.NewGuid());
        await _sut.UpsertAsync(checkList);

        var result = await _sut.GetByIdAsync(checkList.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Created.Should().Be(created);
    }
}
