namespace CM.LiteDB.Tests.Checkables;

public sealed class CheckableDataServiceTests : IDisposable
{
    private readonly LiteDatabase _database = new(":memory:");
    private readonly ServiceProvider _provider;
    private readonly ICheckableDataService _sut;

    public CheckableDataServiceTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILiteDatabase>(_database);
        services.AddLiteDbDataServices();
        _provider = services.BuildServiceProvider();
        _sut = _provider.CreateScope().ServiceProvider.GetRequiredService<ICheckableDataService>();
    }

    public void Dispose()
    {
        _provider.Dispose();
        _database.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_Returns_FailResult_When_Checkable_Is_Not_Found()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task UpsertAsync_And_GetByIdAsync_Persist_And_Retrieve_Checkable()
    {
        var id = Guid.NewGuid();
        var checkListId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var checkable = new Checkable(false, checkListId, DateTimeOffset.UtcNow, "Test task", id, userId);

        await _sut.UpsertAsync(checkable);
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.Description.Should().Be("Test task");
        result.Value.Checked.Should().BeFalse();
        result.Value.CheckListId.Should().Be(checkListId);
        result.Value.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task UpsertAsync_Returns_The_Checkable_On_Insert()
    {
        var checkable = new Checkable(true, Guid.NewGuid(), DateTimeOffset.UtcNow, "Checked task", Guid.NewGuid(), Guid.NewGuid());

        var result = await _sut.UpsertAsync(checkable);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(checkable.Id);
        result.Value.Checked.Should().BeTrue();
    }

    [Fact]
    public async Task UpsertAsync_Updates_Existing_Checkable()
    {
        var id = Guid.NewGuid();
        var original = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Original", id, Guid.NewGuid());
        var updated = new Checkable(true, original.CheckListId, original.Created, "Updated", id, original.UserId);
        await _sut.UpsertAsync(original);

        await _sut.UpsertAsync(updated);
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().Be("Updated");
        result.Value.Checked.Should().BeTrue();
    }

    [Fact]
    public async Task GetByCheckListAsync_Returns_Checkables_For_CheckList()
    {
        var checkListId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var checkable1 = new Checkable(false, checkListId, DateTimeOffset.UtcNow, "Task 1", Guid.NewGuid(), userId);
        var checkable2 = new Checkable(true, checkListId, DateTimeOffset.UtcNow, "Task 2", Guid.NewGuid(), userId);
        var otherCheckable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Other task", Guid.NewGuid(), userId);
        await _sut.UpsertAsync(checkable1);
        await _sut.UpsertAsync(checkable2);
        await _sut.UpsertAsync(otherCheckable);

        var result = await _sut.GetByCheckListAsync(checkListId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(c => c.CheckListId.Should().Be(checkListId));
    }

    [Fact]
    public async Task GetByCheckListAsync_Returns_Empty_List_When_No_Checkables_Exist()
    {
        var result = await _sut.GetByCheckListAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_When_Checkable_Is_Deleted()
    {
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "To delete", Guid.NewGuid(), Guid.NewGuid());
        await _sut.UpsertAsync(checkable);

        var result = await _sut.DeleteAsync(checkable.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Returns_FailResult_When_Checkable_Does_Not_Exist()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task DeleteAsync_Removes_Checkable_So_It_Cannot_Be_Retrieved()
    {
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "To delete", Guid.NewGuid(), Guid.NewGuid());
        await _sut.UpsertAsync(checkable);
        await _sut.DeleteAsync(checkable.Id);

        var result = await _sut.GetByIdAsync(checkable.Id);

        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task UpsertAsync_Preserves_DateTimeOffset_With_Timezone_Offset()
    {
        var created = new DateTimeOffset(2025, 4, 17, 12, 30, 45, 123, TimeSpan.FromHours(2));
        var checkable = new Checkable(false, Guid.NewGuid(), created, "Task", Guid.NewGuid(), Guid.NewGuid());
        await _sut.UpsertAsync(checkable);

        var result = await _sut.GetByIdAsync(checkable.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Created.Should().Be(created);
    }
}
