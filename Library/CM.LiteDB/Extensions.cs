namespace CM.LiteDB;

/// <summary>
/// Extension methods for <c>CM.LiteDB</c> types.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers all LiteDB data service implementations as scoped services.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddLiteDbDataServices(this IServiceCollection services)
    {
        services.AddScoped<ICheckableDataService, Checkables.CheckableDataService>();
        services.AddScoped<ICheckListDataService, CheckLists.CheckListDataService>();
        services.AddScoped<IUserDataService, Users.UserDataService>();

        return services;
    }
}
