namespace CM.LiteDB.Tests;

public sealed class ExtensionsTests
{
    [Fact]
    public void AddLiteDbDataServices_Registers_ICheckableDataService()
    {
        using var database = new LiteDatabase(":memory:");
        var services = new ServiceCollection();
        services.AddSingleton<ILiteDatabase>(database);
        services.AddLiteDbDataServices();
        using var provider = services.BuildServiceProvider();

        var service = provider.CreateScope().ServiceProvider.GetService<ICheckableDataService>();

        service.Should().NotBeNull();
    }

    [Fact]
    public void AddLiteDbDataServices_Registers_ICheckListDataService()
    {
        using var database = new LiteDatabase(":memory:");
        var services = new ServiceCollection();
        services.AddSingleton<ILiteDatabase>(database);
        services.AddLiteDbDataServices();
        using var provider = services.BuildServiceProvider();

        var service = provider.CreateScope().ServiceProvider.GetService<ICheckListDataService>();

        service.Should().NotBeNull();
    }

    [Fact]
    public void AddLiteDbDataServices_Registers_IUserDataService()
    {
        using var database = new LiteDatabase(":memory:");
        var services = new ServiceCollection();
        services.AddSingleton<ILiteDatabase>(database);
        services.AddLiteDbDataServices();
        using var provider = services.BuildServiceProvider();

        var service = provider.CreateScope().ServiceProvider.GetService<IUserDataService>();

        service.Should().NotBeNull();
    }

    [Fact]
    public void AddLiteDbDataServices_Returns_Same_ServiceCollection_For_Chaining()
    {
        using var database = new LiteDatabase(":memory:");
        var services = new ServiceCollection();
        services.AddSingleton<ILiteDatabase>(database);

        var returned = services.AddLiteDbDataServices();

        returned.Should().BeSameAs(services);
    }
}
