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

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<CheckMateHub>("/events");
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.PropertyNameCaseInsensitive = true;
});
app.MapFallbackToFile("index.html");

app.Run();
