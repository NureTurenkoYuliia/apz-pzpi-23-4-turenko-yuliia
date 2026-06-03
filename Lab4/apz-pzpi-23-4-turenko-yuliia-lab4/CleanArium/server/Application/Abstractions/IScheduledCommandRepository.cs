using Domain.Models;

namespace Application.Abstractions;

public interface IScheduledCommandRepository
{
    Task<ScheduledCommand?> GetByIdAsync(long id, CancellationToken ct);
    Task<IEnumerable<ScheduledCommand>> GetAllByDeviceIdAsync(long deviceId);
    Task<ScheduledCommand> AddAsync(ScheduledCommand command);
    Task UpdateAsync(ScheduledCommand command);
    Task DeleteAsync(ScheduledCommand command);
    Task<int> CountByDevice(long deviceId, CancellationToken ct);
    Task<bool> ExistsByIdAsync(long commandId, CancellationToken ct);
    Task<bool> ExistsByUserIdAsync(long userId, long commandId, CancellationToken ct);
}
