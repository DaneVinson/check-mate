namespace CM.Api.Tests;

public sealed class ExtensionsTests
{
    private sealed class SampleOptions
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    private static IConfiguration BuildConfiguration(string json) =>
        new ConfigurationBuilder()
            .AddJsonStream(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            .Build();

    [Fact]
    public void GetConfigObject_Returns_Bound_Object_When_Section_Exists()
    {
        var config = BuildConfiguration("""{ "SampleOptions": { "Name": "Test", "Count": 42 } }""");

        var result = config.GetConfigObject<SampleOptions>();

        result.Name.Should().Be("Test");
        result.Count.Should().Be(42);
    }

    [Fact]
    public void GetConfigObject_Uses_TypeName_As_Default_Path()
    {
        var config = BuildConfiguration("""{ "SampleOptions": { "Name": "Bilbo", "Count": 1 } }""");

        var result = config.GetConfigObject<SampleOptions>();

        result.Name.Should().Be("Bilbo");
    }

    [Fact]
    public void GetConfigObject_Uses_Supplied_ConfigPath_When_Provided()
    {
        var config = BuildConfiguration("""{ "CustomSection": { "Name": "Gandalf", "Count": 99 } }""");

        var result = config.GetConfigObject<SampleOptions>("CustomSection");

        result.Name.Should().Be("Gandalf");
        result.Count.Should().Be(99);
    }

    [Fact]
    public void GetConfigObject_Throws_InvalidOperationException_When_Section_Is_Missing()
    {
        var config = BuildConfiguration("""{ "OtherSection": { "Name": "Frodo" } }""");

        var act = () => config.GetConfigObject<SampleOptions>();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SampleOptions*");
    }
}
