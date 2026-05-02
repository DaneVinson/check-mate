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
    public override async Task HandleAsync(QueryRequest request, CancellationToken cancellationToken)
    {
        var types = QueryTypeResolver.Resolve(request.Type);
        if (types is null)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(new { message = $"Unknown type: {request.Type}" }, cancellationToken);
            return;
        }

        var query = request.Payload.Deserialize(types.Value.QueryType, _options)!;
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(types.Value.QueryType, types.Value.ResultType);
        var handler = (IQueryHandler)HttpContext.RequestServices.GetRequiredService(handlerType);
        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsError)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(new { message = result.Error.Message }, cancellationToken);
            return;
        }

        if (result.Value is null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        await HttpContext.Response.WriteAsJsonAsync(result.Value, cancellationToken);
    }
}
