using Serilog;
using ShopWave.API.Extensions;
using ShopWave.API.Middleware;
using ShopWave.Application;
using ShopWave.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog((ctx, lc) =>
        lc.ReadFrom.Configuration(ctx.Configuration));

    // Application + Infrastructure layers
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // MVC controllers
    builder.Services.AddControllers();

    // OpenAPI (built-in .NET 10) with JWT
    builder.Services.AddOpenApiWithJwt();

    // CORS (open in dev)
    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(policy =>
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

    var app = builder.Build();

    // Auto-migrate on startup
    if (app.Environment.IsDevelopment() ||
        app.Configuration.GetValue<bool>("AutoMigrate"))
    {
        await app.Services.ApplyMigrationsAsync();
        Log.Information("Database migration applied.");
    }

    // Middleware pipeline
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    if (!app.Environment.IsProduction())
        app.UseOpenApiWithScalar();

    app.UseCors();
    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseMiddleware<TenantResolutionMiddleware>();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("ShopWave API starting...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
