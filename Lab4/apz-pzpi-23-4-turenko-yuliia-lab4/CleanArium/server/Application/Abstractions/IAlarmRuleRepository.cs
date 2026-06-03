using Domain.Models;

namespace Application.Abstractions;

public interface IAlarmRuleRepository
{
    Task AddAsync(AlarmRule rule, CancellationToken ct);
    Task UpdateAsync(AlarmRule rule, CancellationToken ct);
    Task DeleteAsync(long id, CancellationToken ct);
    Task<AlarmRule?> GetByIdAsync(long id, CancellationToken ct);
    Task<List<AlarmRule>> GetAllByDeviceIdAsync(long deviceId, CancellationToken ct);
    Task<int> CountByDevice(long deviceId, CancellationToken ct);
    Task<bool> ExistsByIdAsync(long ruleId, CancellationToken ct);
    Task<bool> ExistsByUserIdAsync(long userId, long ruleId, CancellationToken ct);
}
