namespace GenericHostConsoleApp.Services;

internal interface IWeatherService {
  Task<IReadOnlyList<int>> GetFiveDayTemperaturesAsync(CancellationToken cancellationToken);
}
