namespace CM.Domain.Tests.Cqrs;

public sealed class ResultTests
{
    [Fact]
    public void ImplicitConversion_From_Value_Creates_Success_Result()
    {
        Result<int> result = 42;

        result.IsSuccess.Should().BeTrue();
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ImplicitConversion_From_FailResult_Creates_Error_Result()
    {
        Result<int> result = new FailResult("Something went wrong.");

        result.IsError.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Be("Something went wrong.");
    }

    [Fact]
    public void Success_Factory_Creates_Successful_Result()
    {
        var result = Result<string>.Success("hello");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void Fail_Factory_Creates_Error_Result()
    {
        var error = new FailResult("error message");

        var result = Result<string>.Fail(error);

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Be("error message");
    }

    [Fact]
    public void Value_Throws_InvalidOperationException_When_Result_Is_Error()
    {
        Result<int> result = new FailResult("error");

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Error_Throws_InvalidOperationException_When_Result_Is_Success()
    {
        Result<int> result = 42;

        var act = () => result.Error;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void FailResult_Message_Returns_Correct_Value()
    {
        var failResult = new FailResult("Test error message");

        failResult.Message.Should().Be("Test error message");
    }

    [Fact]
    public void Success_Factory_Wraps_Interface_Type_Without_Implicit_Operator()
    {
        IReadOnlyList<int> list = new List<int> { 1, 2, 3 };

        var result = Result<IReadOnlyList<int>>.Success(list);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public void Fail_Factory_Wraps_Interface_Type_Without_Implicit_Operator()
    {
        var error = new FailResult("interface type error");

        var result = Result<IReadOnlyList<int>>.Fail(error);

        result.IsError.Should().BeTrue();
        result.Error.Message.Should().Be("interface type error");
    }
}
