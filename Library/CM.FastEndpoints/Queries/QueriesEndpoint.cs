namespace CM.FastEndpoints.Queries;

/// <summary>
/// Dispatches queries to their handlers via POST /queries.
/// </summary>
internal sealed class QueriesEndpoint : global::FastEndpoints.Endpoint<QueryRequest>
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueriesEndpoint"/> class.
    /// </summary>
    public QueriesEndpoint(JsonSerializerOptions options)
    {
        _options = options;
    }

    /// <inheritdoc />
    public override void Configure()
    {
        Post("/queries");
        AllowAnonymous();
    }

    /// <inheritdoc />
    public override async Task HandleAsync(QueryRequest req, CancellationToken ct)
    {
        var types = ResolveQueryTypes(req.Type);
        if (types is null)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(new { message = $"Unknown type: {req.Type}" }, ct);
            return;
        }

        var query = req.Payload.Deserialize(types.Value.QueryType, _options)!;
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(types.Value.QueryType, types.Value.ResultType);
        var handler = HttpContext.RequestServices.GetRequiredService(handlerType);
        var task = (Task)handlerType.GetMethod("HandleAsync")!.Invoke(handler, [query, ct])!;
        await task;

        var resultObj = task.GetType().GetProperty("Result")!.GetValue(task)!;
        var resultType = resultObj.GetType();

        if ((bool)resultType.GetProperty("IsError")!.GetValue(resultObj)!)
        {
            var error = resultType.GetProperty("Error")!.GetValue(resultObj)!;
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(new { message = error.GetType().GetProperty("Message")!.GetValue(error) }, ct);
            return;
        }

        var value = resultType.GetProperty("Value")!.GetValue(resultObj);
        if (value is null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        await HttpContext.Response.WriteAsJsonAsync(value, ct);
    }

    private static (Type QueryType, Type ResultType)? ResolveQueryTypes(string type) => type switch
    {
        "GetCheckablesByCheckList" => (typeof(GetCheckablesByCheckList), typeof(IReadOnlyList<CheckableDto>)),
        "GetCheckList"             => (typeof(GetCheckList), typeof(CheckListDto)),
        "GetCheckListsByUser"      => (typeof(GetCheckListsByUser), typeof(IReadOnlyList<CheckListDto>)),
        "GetUser"                  => (typeof(GetUser), typeof(UserDto)),
        _                          => null
    };
}
