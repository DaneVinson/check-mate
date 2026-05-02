namespace CM.FastEndpoints.Commands;

/// <summary>
/// Resolves command type names to their corresponding <see cref="Type"/> instances.
/// </summary>
public static class CommandTypeResolver
{
    /// <summary>
    /// Returns the <see cref="Type"/> for the given command type name, or <see langword="null"/> if unknown.
    /// </summary>
    /// <param name="type">The command type name as used in the request payload.</param>
    public static Type? Resolve(string type) => type switch
    {
        "CheckCheckable" => typeof(CheckCheckable),
        "CreateCheckable" => typeof(CreateCheckable),
        "CreateCheckList" => typeof(CreateCheckList),
        "CreateUser" => typeof(CreateUser),
        "DeleteCheckable" => typeof(DeleteCheckable),
        "DeleteCheckList" => typeof(DeleteCheckList),
        "UncheckCheckable" => typeof(UncheckCheckable),
        "UpdateCheckable" => typeof(UpdateCheckable),
        "UpdateCheckList" => typeof(UpdateCheckList),
        "UpdateUser" => typeof(UpdateUser),
        _ => null
    };
}
