using FraudEngine.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.AddSerilog();
builder.Services.AddApplicationDependencies(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Apply migrations
await app.ApplyMigrationsAsync();

// Configure pipeline
app.UseApplicationMiddleware();
app.UseSwaggerDocumentation();
app.MapApiEndpoints();

app.Run();

public partial class Program { }
