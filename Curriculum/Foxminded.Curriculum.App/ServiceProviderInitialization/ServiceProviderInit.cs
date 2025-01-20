using Foxminded.Curriculum.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;

namespace Foxminded.Curriculum.App.ServiceProviderInitialization;

public static class ServiceProviderInit
{
    public static ServiceProvider GetServiceProvider(this IServiceCollection services)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Prod";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json")
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        var connectionString = configuration.GetConnectionString("DB");

        return services
               .AddSingleton<IConfiguration>(configuration)
               .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
               .AddTransient<IDbUnitOfWorkFactory>(_ => new DbUnitOfWorkFactory(connectionString!))
               .GetServicesTransient()
               .AddSingleton<MainWindow>(s => new MainWindow())
               .BuildServiceProvider();
    }
}
