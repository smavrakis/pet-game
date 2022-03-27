using PetGame.Configuration;
using PetGame.Middleware;
using PetGame.Persistence;
using PetGame.Persistence.Configuration;
using Serilog;
using Serilog.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

try
{
    Log.Information("Building application and services...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.ConfigureAppConfiguration(options => options.AddEnvironmentVariables());
    builder.Host.UseSerilog();

    ConfigureServices(builder.Services, builder.Configuration);

    var app = builder.Build();
    ConfigureApp(app);

    Log.Information("Starting application...");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}


static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton(Log.Logger);
    services.AddControllers(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;        
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    services.AddApiServices(configuration);
    services.AddPersistenceServices(configuration);
}

static void ConfigureApp(WebApplication app)
{
    // This is just for testing and a POC of the game, for production we want to use migrations instead
    Log.Information("Building database...");
    using (var scope = app.Services.CreateScope())
    {
        var databaseContext = scope.ServiceProvider.GetService<GameContext>() ?? throw new InvalidOperationException("Could not resolve database context");
        databaseContext.Database.EnsureCreated();
    }

    app.UseSerilogRequestLogging();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.MapControllers();
}

// Required for integration tests
public partial class Program { }
