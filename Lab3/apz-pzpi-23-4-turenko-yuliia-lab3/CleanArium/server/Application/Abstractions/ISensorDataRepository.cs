namespace Application.Abstractions;

public interface ISensorDataRepository
{
    Task AddAsync(Domain.Models.SensorData data);
    Task<Domain.Models.SensorData?> GetLatestByDeviceAsync(long deviceId);
    Task<List<Domain.Models.SensorData>> GetByDeviceAsync(long deviceId, DateTime from, DateTime to);
    Task<List<Domain.Models.SensorData>> GetRecentAsync(long deviceId, TimeSpan interval);
    Task<double?> GetAverageValueAsync(long deviceId, DateTime from, DateTime to);
    Task<double?> GetTrendSlopeAsync(long deviceId, DateTime from, DateTime to);
}
