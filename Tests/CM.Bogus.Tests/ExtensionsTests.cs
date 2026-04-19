namespace CM.Bogus.Tests;

public sealed class ExtensionsTests
{
    [Fact]
    public void AddBogusDataServices_Registers_ICheckableDataService()
    {
        var services = new ServiceCollection();
        services.AddBogusDataServices();
        using var provider = services.BuildServiceProvider();

        var service = provider.CreateScope().ServiceProvider.GetService<ICheckableDataService>();

        service.Should().NotBeNull();
    }

    [Fact]
    public void AddBogusDataServices_Registers_ICheckListDataService()
    {
        var services = new ServiceCollection();
        services.AddBogusDataServices();
        using var provider = services.BuildServiceProvider();

        var service = provider.CreateScope().ServiceProvider.GetService<ICheckListDataService>();

        service.Should().NotBeNull();
    }

    [Fact]
    public void AddBogusDataServices_Registers_IUserDataService()
    {
        var services = new ServiceCollection();
        services.AddBogusDataServices();
        using var provider = services.BuildServiceProvider();

        var service = provider.CreateScope().ServiceProvider.GetService<IUserDataService>();

        service.Should().NotBeNull();
    }

    [Fact]
    public void AddBogusDataServices_Returns_Same_ServiceCollection_For_Chaining()
    {
        var services = new ServiceCollection();

        var returned = services.AddBogusDataServices();

        returned.Should().BeSameAs(services);
    }
}
