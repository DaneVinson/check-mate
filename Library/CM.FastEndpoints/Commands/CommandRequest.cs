namespace CM.FastEndpoints.Commands;

/// <summary>
/// The HTTP request wrapper for command dispatch.
/// </summary>
/// <param name="Payload">The serialized command payload.</param>
/// <param name="Type">The command type name.</param>
public record CommandRequest(System.Text.Json.JsonElement Payload, string Type);
