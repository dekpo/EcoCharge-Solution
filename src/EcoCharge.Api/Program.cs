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

// ---- Database migrations on startup ---------------------------------------
// Apply pending EF Core migrations so the schema stays in source control
// (Infrastructure/Persistence/Migrations). If a SQLite file was previously
// created with EnsureCreated, delete it (or the Docker volume) once —
// otherwise MigrateAsync will fail because tables already exist without a
// __EFMigrationsHistory row.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    var seederLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger(typeof(DemoStationSeeder));
    await DemoStationSeeder.SeedIfEmptyAsync(dbContext, seederLogger);
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
