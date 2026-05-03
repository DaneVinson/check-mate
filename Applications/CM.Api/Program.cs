var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
builder.Services.AddDefaultHandlers();
builder.Services.AddBogusDataServices();
builder.Services.AddSignalRMessenger();
builder.Services.AddFastEndpoints();

var jwtOptions = builder.Configuration.GetConfigObject<JwtOptions>();
builder.Services.AddSingleton(jwtOptions);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) && context.HttpContext.Request.Path.StartsWithSegments("/events"))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidAudience = jwtOptions.Audience,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

var quasarRoot = Path.Combine(app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot"), "quasar");
var contentTypeProvider = new FileExtensionContentTypeProvider();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<CheckMateHub>("/events");
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.PropertyNameCaseInsensitive = true;
});
app.MapGet("/quasar/{**path}", async (string? path, HttpContext ctx) =>
{
    var relativePath = string.IsNullOrEmpty(path) ? "index.html" : path;
    var fullPath = Path.GetFullPath(Path.Combine(quasarRoot, relativePath));

    if (!fullPath.StartsWith(quasarRoot, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
    {
        ctx.Response.ContentType = "text/html";
        await ctx.Response.SendFileAsync(Path.Combine(quasarRoot, "index.html"));
        return;
    }

    contentTypeProvider.TryGetContentType(fullPath, out var ct);
    ctx.Response.ContentType = ct ?? "application/octet-stream";
    await ctx.Response.SendFileAsync(fullPath);
});

app.Run();
