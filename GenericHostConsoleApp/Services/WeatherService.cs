using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GenericHostConsoleApp.Services;

internal sealed class WeatherService : IWeatherService {
  private readonly ILogger<WeatherService> logger;
  private readonly IOptions<WeatherSettings> weatherSettings;

  public WeatherService(
    ILogger<WeatherService> logger,
    IOptions<WeatherSettings> weatherSettings) {
    this.logger = logger;
    this.weatherSettings = weatherSettings;
  }

  public async Task<IReadOnlyList<int>> GetFiveDayTemperaturesAsync(CancellationToken cancellationToken) {
    logger.LogInformation("Fetching weather...");

    // Simulate some network latency
    await Task.Delay(2000, cancellationToken);

    int[] temperatures = new[] { 76, 76, 77, 79, 78 };
    if (weatherSettings.Value.Unit.Equals("C", StringComparison.OrdinalIgnoreCase)) {
      for (int i = 0; i < temperatures.Length; i++) {
        temperatures[i] = (int)Math.Round((temperatures[i] - 32) / 1.8);
      }
    }

    logger.LogInformation("Fetched weather successfully");
    return temperatures;
  }
}