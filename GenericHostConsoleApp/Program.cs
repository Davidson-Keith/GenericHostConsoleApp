using System.Reflection;
using GenericHostConsoleApp;
using GenericHostConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/**
 * A Console App Hosted by the .NET Generic Host.
 * Using the Generic Host's built in:
 *  - DI
 *  - Configuration
 *  - Logging  
 * 
 * From:
 * https://dfederm.com/building-a-console-app-with-.net-generic-host/
 * https://github.com/dfederm/GenericHostConsoleApp/blob/main/appsettings.json
 */
internal sealed class Program {
  private static async Task Main(string[] args) {
    await Host.CreateDefaultBuilder(args)
        .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
        .ConfigureLogging(logging => {
          // Add any 3rd party loggers like NLog or Serilog
        })
        .ConfigureServices((hostContext, services) => {
          services
            .AddHostedService<ConsoleHostedService>()
            .AddSingleton<IWeatherService, WeatherService>();

          services.AddOptions<WeatherSettings>().Bind(hostContext.Configuration.GetSection("Weather"));
        })
        .RunConsoleAsync();
  }
}
