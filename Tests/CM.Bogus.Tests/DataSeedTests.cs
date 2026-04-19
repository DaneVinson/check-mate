namespace CM.Bogus.Tests;

public sealed class DataSeedTests
{
    [Fact]
    public void GetUsers_Returns_Exactly_One_User()
    {
        var users = DataSeed.GetUsers();

        users.Should().ContainSingle();
    }

    [Fact]
    public void GetUsers_Returns_Bilbo_Baggins_With_Correct_Email()
    {
        var user = DataSeed.GetUsers().Single();

        user.Name.Should().Be("Bilbo Baggins");
        user.Email.Should().Be("bilbo.baggins@shire.me");
    }

    [Fact]
    public void GetUsers_Returns_User_With_Fixed_Id()
    {
        var user = DataSeed.GetUsers().Single();

        user.Id.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000001"));
    }

    [Fact]
    public void GetCheckLists_Returns_Ten_CheckLists()
    {
        var checkLists = DataSeed.GetCheckLists();

        checkLists.Should().HaveCount(10);
    }

    [Fact]
    public void GetCheckLists_All_Belong_To_Seeded_User()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var checkLists = DataSeed.GetCheckLists();

        checkLists.Should().AllSatisfy(cl => cl.UserId.Should().Be(userId));
    }

    [Fact]
    public void GetCheckLists_All_Have_Non_Empty_Names()
    {
        var checkLists = DataSeed.GetCheckLists();

        checkLists.Should().AllSatisfy(cl => cl.Name.Should().NotBeNullOrWhiteSpace());
    }

    [Fact]
    public void GetCheckables_Returns_At_Least_Thirty_Items()
    {
        var checkables = DataSeed.GetCheckables();

        checkables.Count.Should().BeGreaterThanOrEqualTo(30);
    }

    [Fact]
    public void GetCheckables_All_Have_Valid_CheckListId()
    {
        var checkListIds = DataSeed.GetCheckLists().Select(cl => cl.Id).ToHashSet();

        var checkables = DataSeed.GetCheckables();

        checkables.Should().AllSatisfy(c => checkListIds.Should().Contain(c.CheckListId));
    }

    [Fact]
    public void GetCheckables_All_Have_Non_Empty_Descriptions()
    {
        var checkables = DataSeed.GetCheckables();

        checkables.Should().AllSatisfy(c => c.Description.Should().NotBeNullOrWhiteSpace());
    }

    [Fact]
    public void GetCheckables_Returns_Deterministic_Data_On_Repeated_Calls()
    {
        var first = DataSeed.GetCheckables().Select(c => c.Id).ToList();
        var second = DataSeed.GetCheckables().Select(c => c.Id).ToList();

        first.Should().Equal(second);
    }

    [Fact]
    public void GetCheckLists_Returns_Deterministic_Data_On_Repeated_Calls()
    {
        var first = DataSeed.GetCheckLists().Select(cl => cl.Id).ToList();
        var second = DataSeed.GetCheckLists().Select(cl => cl.Id).ToList();

        first.Should().Equal(second);
    }
}
