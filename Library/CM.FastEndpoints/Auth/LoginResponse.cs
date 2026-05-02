namespace CM.FastEndpoints.Auth;

/// <summary>
/// Represents the JWT access token response returned by the login endpoint.
/// </summary>
/// <param name="AccessToken">The JWT access token string.</param>
/// <param name="ExpiresIn">The lifetime of the token in seconds.</param>
/// <param name="TokenType">The token type, always <c>Bearer</c>.</param>
public record LoginResponse(string AccessToken, int ExpiresIn, string TokenType);
