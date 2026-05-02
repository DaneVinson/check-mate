namespace CM.FastEndpoints.Tests.Auth;

public sealed class JwtOptionsTests
{
    [Fact]
    public void Audience_Returns_Default_Value()
    {
        var sut = new JwtOptions();

        sut.Audience.Should().Be("checkmate-spa");
    }

    [Fact]
    public void ExpiryMinutes_Returns_Default_Value()
    {
        var sut = new JwtOptions();

        sut.ExpiryMinutes.Should().Be(60);
    }

    [Fact]
    public void Issuer_Returns_Default_Value()
    {
        var sut = new JwtOptions();

        sut.Issuer.Should().Be("https://checkmate.local");
    }

    [Fact]
    public void SigningKey_Returns_Empty_String_By_Default()
    {
        var sut = new JwtOptions();

        sut.SigningKey.Should().BeEmpty();
    }
}
