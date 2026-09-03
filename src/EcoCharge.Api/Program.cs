using EcoCharge.Api.Common.ExceptionHandlers;
using EcoCharge.Api.Endpoints;
using EcoCharge.Application;
using EcoCharge.Infrastructure;
using EcoCharge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string ClientCorsPolicy = "Client";

// ---- Dependency injection -------------------------------------------------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

// Global exception handling -> standardized ProblemDetails JSON responses.
// Handlers run in registration order; the fallback must be registered last.
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<FallbackExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS: only the configured frontend origin is allowed.
var allowedOrigin = builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy(ClientCorsPolicy, policy => policy
        .WithOrigins(allowedOrigin)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// ---- Database provisioning on startup -------------------------------------
// NOTE: no EF Core migration has been generated yet (requires a real `dotnet
// ef migrations add InitialCreate`, which needs NuGet restore to have run at
// least once - see README "Getting the backend running"). Until then,
// EnsureCreatedAsync provisions the schema directly from the model, which is
// enough for local/demo use. Switch to `await dbContext.Database.MigrateAsync()`
// once a migrations history exists, so schema changes are tracked properly.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// ---- Middleware pipeline ---------------------------------------------------
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(ClientCorsPolicy);

app.MapChargingStationEndpoints();
app.MapChargeSessionEndpoints();

app.MapGet("/health", () => TypedResults.Ok(new { status = "healthy" })).WithTags("Health");

app.Run();
