namespace CM.FastEndpoints.Tests.Commands;

public sealed class CommandTypeResolverTests
{
    [Theory]
    [InlineData("CheckCheckable", typeof(CheckCheckable))]
    [InlineData("CreateCheckable", typeof(CreateCheckable))]
    [InlineData("CreateCheckList", typeof(CreateCheckList))]
    [InlineData("CreateUser", typeof(CreateUser))]
    [InlineData("DeleteCheckable", typeof(DeleteCheckable))]
    [InlineData("DeleteCheckList", typeof(DeleteCheckList))]
    [InlineData("UncheckCheckable", typeof(UncheckCheckable))]
    [InlineData("UpdateCheckable", typeof(UpdateCheckable))]
    [InlineData("UpdateCheckList", typeof(UpdateCheckList))]
    [InlineData("UpdateUser", typeof(UpdateUser))]
    public void Resolve_Returns_Correct_Type_For_Known_Command(string typeName, Type expectedType)
    {
        var result = CommandTypeResolver.Resolve(typeName);

        result.Should().Be(expectedType);
    }

    [Fact]
    public void Resolve_Returns_Null_For_Unknown_Type()
    {
        var result = CommandTypeResolver.Resolve("UnknownCommand");

        result.Should().BeNull();
    }
}
