using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
builder.Services.AddDefaultHandlers();
builder.Services.AddBogusDataServices();
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints(c =>
{
    c.Serializer.Options.PropertyNameCaseInsensitive = true;
});

app.Run();
