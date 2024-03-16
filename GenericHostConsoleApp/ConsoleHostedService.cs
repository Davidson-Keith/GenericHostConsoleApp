using GenericHostConsoleApp.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenericHostConsoleApp;

internal sealed class ConsoleHostedService : IHostedService
{
    private readonly ILogger logger;
    private readonly IHostApplicationLifetime appLifetime;
    private readonly IWeatherService weatherService;

    private Task? applicationTask;
    private int? exitCode;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime,
        IWeatherService weatherService)
    {
        this.logger = logger;
        this.appLifetime = appLifetime;
        this.weatherService = weatherService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

        CancellationTokenSource? cancellationTokenSource = null;

        appLifetime.ApplicationStarted.Register(() =>
        {
            logger.LogDebug("Application has started");
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            applicationTask = Task.Run(async () =>
            {
                try
                {
                    IReadOnlyList<int> temperatures = await weatherService.GetFiveDayTemperaturesAsync(cancellationTokenSource.Token);
                    for (int i = 0; i < temperatures.Count; i++)
                    {
                        logger.LogInformation($"{DateTime.Today.AddDays(i).DayOfWeek}: {temperatures[i]}");
                    }

                    exitCode = 0;
                }
                catch (TaskCanceledException)
                {
                    // This means the application is shutting down, so just swallow this exception
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unhandled exception!");
                    exitCode = 1;
                }
                finally
                {
                    // Stop the application once the work is done
                    appLifetime.StopApplication();
                }
            });
        });

        appLifetime.ApplicationStopping.Register(() =>
        {
            logger.LogDebug("Application is stopping");
            cancellationTokenSource?.Cancel();
        });

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Wait for the application logic to fully complete any cleanup tasks.
        // Note that this relies on the cancellation token to be properly used in the application.
        if (applicationTask != null)
        {
            await applicationTask;
        }

        logger.LogDebug($"Exiting with return code: {exitCode}");

        // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
        Environment.ExitCode = exitCode.GetValueOrDefault(-1);
    }
}