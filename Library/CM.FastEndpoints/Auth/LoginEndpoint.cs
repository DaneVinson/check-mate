namespace CM.FastEndpoints.Auth;

/// <summary>
/// Issues OIDC-compliant JWT access tokens via POST /auth/token.
/// </summary>
internal sealed class LoginEndpoint : global::FastEndpoints.Endpoint<LoginRequest>
{
    private readonly JwtOptions _jwtOptions;
    private readonly IUserDataService _userDataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginEndpoint"/> class.
    /// </summary>
    /// <param name="jwtOptions">The JWT options.</param>
    /// <param name="userDataService">The user data service.</param>
    public LoginEndpoint(JwtOptions jwtOptions, IUserDataService userDataService)
    {
        _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
        _userDataService = userDataService ?? throw new ArgumentNullException(nameof(userDataService));
    }

    /// <inheritdoc />
    public override void Configure()
    {
        Post("/auth/token");
        AllowAnonymous();
    }

    /// <inheritdoc />
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _userDataService.GetByEmailAsync(req.Email);

        if (result.IsError)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var user = result.Value;

        if (user is null || !user.Name.Equals(req.Name, StringComparison.OrdinalIgnoreCase))
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        await HttpContext.Response.WriteAsJsonAsync(GenerateToken(user), ct);
    }

    private LoginResponse GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expiry = now.AddMinutes(_jwtOptions.ExpiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim("userId", user.Id.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtOptions.Audience,
            Expires = expiry,
            Issuer = _jwtOptions.Issuer,
            SigningCredentials = credentials,
            Subject = new ClaimsIdentity(claims),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginResponse(tokenHandler.WriteToken(token), _jwtOptions.ExpiryMinutes * 60, "Bearer");
    }
}
