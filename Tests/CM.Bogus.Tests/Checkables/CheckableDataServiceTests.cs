namespace CM.Bogus.Tests.Checkables;

public sealed class CheckableDataServiceTests
{
    private static ICheckableDataService CreateService()
    {
        var services = new ServiceCollection();
        services.AddBogusDataServices();
        var provider = services.BuildServiceProvider();
        return provider.CreateScope().ServiceProvider.GetRequiredService<ICheckableDataService>();
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Checkable_When_Id_Exists()
    {
        var seededCheckable = DataSeed.GetCheckables().First();
        var sut = CreateService();

        var result = await sut.GetByIdAsync(seededCheckable.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(seededCheckable.Id);
        result.Value.Description.Should().Be(seededCheckable.Description);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_FailResult_When_Id_Does_Not_Exist()
    {
        var sut = CreateService();

        var result = await sut.GetByIdAsync(Guid.NewGuid());

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetByCheckListAsync_Returns_Checkables_For_Known_CheckList()
    {
        var seededCheckList = DataSeed.GetCheckLists().First();
        var expectedCount = DataSeed.GetCheckables().Count(c => c.CheckListId == seededCheckList.Id);
        var sut = CreateService();

        var result = await sut.GetByCheckListAsync(seededCheckList.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task GetByCheckListAsync_Returns_Empty_List_When_CheckList_Has_No_Checkables()
    {
        var sut = CreateService();

        var result = await sut.GetByCheckListAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task UpsertAsync_Returns_Checkable_When_Inserted()
    {
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "New test task", Guid.NewGuid(), Guid.NewGuid());
        var sut = CreateService();

        var result = await sut.UpsertAsync(checkable);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(checkable.Id);
        result.Value.Description.Should().Be(checkable.Description);
    }

    [Fact]
    public async Task UpsertAsync_Updates_Existing_Checkable()
    {
        var checkableId = Guid.NewGuid();
        var original = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Original bogus task", checkableId, Guid.NewGuid());
        var updated = new Checkable(true, original.CheckListId, original.Created, "Updated bogus task", checkableId, original.UserId);
        var sut = CreateService();
        await sut.UpsertAsync(original);

        await sut.UpsertAsync(updated);
        var result = await sut.GetByIdAsync(checkableId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().Be("Updated bogus task");
        result.Value.Checked.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_When_Checkable_Exists()
    {
        var checkable = new Checkable(false, Guid.NewGuid(), DateTimeOffset.UtcNow, "Task to delete", Guid.NewGuid(), Guid.NewGuid());
        var sut = CreateService();
        await sut.UpsertAsync(checkable);

        var result = await sut.DeleteAsync(checkable.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Returns_FailResult_When_Checkable_Does_Not_Exist()
    {
        var sut = CreateService();

        var result = await sut.DeleteAsync(Guid.NewGuid());

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }
}
