namespace CM.FastEndpoints.Commands;

/// <summary>
/// Dispatches commands to their handlers via POST /commands.
/// </summary>
internal sealed class CommandsEndpoint : global::FastEndpoints.Endpoint<CommandRequest>
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandsEndpoint"/> class.
    /// </summary>
    public CommandsEndpoint(JsonSerializerOptions options)
    {
        _options = options;
    }

    /// <inheritdoc />
    public override void Configure()
    {
        Post("/commands");
        AllowAnonymous();
    }

    /// <inheritdoc />
    public override async Task HandleAsync(CommandRequest req, CancellationToken ct)
    {
        var commandType = ResolveCommandType(req.Type);
        if (commandType is null)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(new { message = $"Unknown type: {req.Type}" }, ct);
            return;
        }

        var command = req.Payload.Deserialize(commandType, _options)!;
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        var handler = HttpContext.RequestServices.GetRequiredService(handlerType);
        await (Task)handlerType.GetMethod("HandleAsync")!.Invoke(handler, [command, ct])!;
    }

    private static Type? ResolveCommandType(string type) => type switch
    {
        "CheckCheckable"  => typeof(CheckCheckable),
        "CreateCheckable" => typeof(CreateCheckable),
        "CreateCheckList" => typeof(CreateCheckList),
        "CreateUser"      => typeof(CreateUser),
        "DeleteCheckable" => typeof(DeleteCheckable),
        "DeleteCheckList" => typeof(DeleteCheckList),
        "UncheckCheckable" => typeof(UncheckCheckable),
        "UpdateCheckable" => typeof(UpdateCheckable),
        "UpdateCheckList" => typeof(UpdateCheckList),
        "UpdateUser"      => typeof(UpdateUser),
        _                 => null
    };
}
