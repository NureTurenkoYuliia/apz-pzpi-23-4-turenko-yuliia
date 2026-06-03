using Domain.Models;

namespace Application.Abstractions;

public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(long id);
    Task<List<Device>> GetByAquariumIdAsync(long aquariumId);
    Task AddAsync(Device device);
    Task UpdateAsync(Device device);
    Task DeleteAsync(Device device);
    Task<bool> ExistsByIdAsync(long deviceId, CancellationToken ct);
    Task<bool> UserOwnsDeviceAsync(long userId, long deviceId, CancellationToken ct);
    Task<int> CountByAquarium(long aquariumId, CancellationToken ct);
}
