namespace CM.SignalR;

/// <summary>
/// Extension methods for registering CM.SignalR services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers SignalR, <see cref="CheckMateHub"/>, and <see cref="IMessenger{TMessage}"/> of
    /// <see cref="IEvent"/> backed by <see cref="EventMessenger"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddSignalRMessenger(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSignalR();
        services.AddScoped<IMessenger<IEvent>, EventMessenger>();
        return services;
    }
}
