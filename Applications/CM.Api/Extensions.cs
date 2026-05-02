namespace CM.Api;

/// <summary>
/// Extension methods for <see cref="IConfiguration"/>.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Binds a configuration section to an instance of <typeparamref name="T"/>.
    /// The section path defaults to <c>typeof(T).Name</c> when <paramref name="configPath"/> is not supplied.
    /// </summary>
    /// <typeparam name="T">The type to bind the configuration section to.</typeparam>
    /// <param name="configuration">The configuration to read from.</param>
    /// <param name="configPath">The configuration section path. Defaults to <c>typeof(T).Name</c>.</param>
    /// <returns>A populated instance of <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the configuration section is missing or cannot be bound to <typeparamref name="T"/>.
    /// </exception>
    public static T GetConfigObject<T>(this IConfiguration configuration, string? configPath = null)
    {
        var path = configPath ?? typeof(T).Name;
        return configuration.GetSection(path).Get<T>()
            ?? throw new InvalidOperationException($"Configuration section '{path}' is missing or cannot be bound to '{typeof(T).Name}'.");
    }
}
