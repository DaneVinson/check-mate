namespace CM.FastEndpoints.Queries;

/// <summary>
/// The HTTP request wrapper for query dispatch.
/// </summary>
/// <param name="Payload">The serialized query payload.</param>
/// <param name="Type">The query type name.</param>
public record QueryRequest(System.Text.Json.JsonElement Payload, string Type);
