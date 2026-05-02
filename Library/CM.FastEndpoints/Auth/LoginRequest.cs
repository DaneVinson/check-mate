namespace CM.FastEndpoints.Auth;

/// <summary>
/// Represents the credentials submitted to the login endpoint.
/// </summary>
/// <param name="Email">The email address of the user.</param>
/// <param name="Name">The display name of the user.</param>
public record LoginRequest(string Email, string Name);
