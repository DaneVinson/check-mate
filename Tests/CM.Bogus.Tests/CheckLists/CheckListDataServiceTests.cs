namespace CM.Bogus.Tests.CheckLists;

public sealed class CheckListDataServiceTests
{
    private static ICheckListDataService CreateService()
    {
        var services = new ServiceCollection();
        services.AddBogusDataServices();
        var provider = services.BuildServiceProvider();
        return provider.CreateScope().ServiceProvider.GetRequiredService<ICheckListDataService>();
    }

    [Fact]
    public async Task GetByIdAsync_Returns_CheckList_When_Id_Exists()
    {
        var seededCheckList = DataSeed.GetCheckLists().First();
        var sut = CreateService();

        var result = await sut.GetByIdAsync(seededCheckList.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(seededCheckList.Id);
        result.Value.Name.Should().Be(seededCheckList.Name);
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
    public async Task GetByUserAsync_Returns_Ten_CheckLists_For_Seeded_User()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var sut = CreateService();

        var result = await sut.GetByUserAsync(userId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetByUserAsync_Returns_Empty_List_For_Unknown_User()
    {
        var sut = CreateService();

        var result = await sut.GetByUserAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task UpsertAsync_Returns_CheckList_When_Inserted()
    {
        var checkList = new CheckList(DateTimeOffset.UtcNow, Guid.NewGuid(), "New Bogus List", Guid.NewGuid());
        var sut = CreateService();

        var result = await sut.UpsertAsync(checkList);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(checkList.Id);
        result.Value.Name.Should().Be(checkList.Name);
    }

    [Fact]
    public async Task UpsertAsync_Updates_Existing_CheckList()
    {
        var checkListId = Guid.NewGuid();
        var original = new CheckList(DateTimeOffset.UtcNow, checkListId, "Original bogus name", Guid.NewGuid());
        var updated = new CheckList(original.Created, checkListId, "Updated bogus name", original.UserId);
        var sut = CreateService();
        await sut.UpsertAsync(original);

        await sut.UpsertAsync(updated);
        var result = await sut.GetByIdAsync(checkListId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Updated bogus name");
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_When_CheckList_Exists()
    {
        var checkList = new CheckList(DateTimeOffset.UtcNow, Guid.NewGuid(), "List to delete", Guid.NewGuid());
        var sut = CreateService();
        await sut.UpsertAsync(checkList);

        var result = await sut.DeleteAsync(checkList.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Returns_FailResult_When_CheckList_Does_Not_Exist()
    {
        var sut = CreateService();

        var result = await sut.DeleteAsync(Guid.NewGuid());

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }
}
