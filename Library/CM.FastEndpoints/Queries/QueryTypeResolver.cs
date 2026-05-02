namespace CM.FastEndpoints.Queries;

/// <summary>
/// Resolves query type names to their corresponding query and result <see cref="Type"/> instances.
/// </summary>
public static class QueryTypeResolver
{
    /// <summary>
    /// Returns the query and result types for the given query type name, or <see langword="null"/> if unknown.
    /// </summary>
    /// <param name="type">The query type name as used in the request payload.</param>
    public static (Type QueryType, Type ResultType)? Resolve(string type) => type switch
    {
        "GetCheckablesByCheckList" => (typeof(GetCheckablesByCheckList), typeof(IReadOnlyList<CheckableDto>)),
        "GetCheckList" => (typeof(GetCheckList), typeof(CheckListDto)),
        "GetCheckListsByUser" => (typeof(GetCheckListsByUser), typeof(IReadOnlyList<CheckListDto>)),
        "GetUser" => (typeof(GetUser), typeof(UserDto)),
        _ => null
    };
}
