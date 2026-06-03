namespace Application.Abstractions;

public interface ISensorDataService
{
    Task SaveAsync(Domain.Models.SensorData data);
}
