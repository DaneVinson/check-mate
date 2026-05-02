namespace CM.FastEndpoints.Tests.Queries;

public sealed class QueryTypeResolverTests
{
    [Fact]
    public void Resolve_Returns_Null_For_Unknown_Type()
    {
        var result = QueryTypeResolver.Resolve("UnknownQuery");

        result.Should().BeNull();
    }

    [Fact]
    public void Resolve_Returns_Correct_Types_For_GetCheckablesByCheckList()
    {
        var result = QueryTypeResolver.Resolve("GetCheckablesByCheckList");

        result.Should().NotBeNull();
        result!.Value.QueryType.Should().Be(typeof(GetCheckablesByCheckList));
        result!.Value.ResultType.Should().Be(typeof(IReadOnlyList<CheckableDto>));
    }

    [Fact]
    public void Resolve_Returns_Correct_Types_For_GetCheckList()
    {
        var result = QueryTypeResolver.Resolve("GetCheckList");

        result.Should().NotBeNull();
        result!.Value.QueryType.Should().Be(typeof(GetCheckList));
        result!.Value.ResultType.Should().Be(typeof(CheckListDto));
    }

    [Fact]
    public void Resolve_Returns_Correct_Types_For_GetCheckListsByUser()
    {
        var result = QueryTypeResolver.Resolve("GetCheckListsByUser");

        result.Should().NotBeNull();
        result!.Value.QueryType.Should().Be(typeof(GetCheckListsByUser));
        result!.Value.ResultType.Should().Be(typeof(IReadOnlyList<CheckListDto>));
    }

    [Fact]
    public void Resolve_Returns_Correct_Types_For_GetUser()
    {
        var result = QueryTypeResolver.Resolve("GetUser");

        result.Should().NotBeNull();
        result!.Value.QueryType.Should().Be(typeof(GetUser));
        result!.Value.ResultType.Should().Be(typeof(UserDto));
    }
}
