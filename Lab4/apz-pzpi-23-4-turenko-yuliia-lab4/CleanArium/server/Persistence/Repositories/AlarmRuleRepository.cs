using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class AlarmRuleRepository : IAlarmRuleRepository
{
    private readonly CleanAriumDbContext _db;

    public AlarmRuleRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AlarmRule rule, CancellationToken ct)
    {
        await _db.AlarmRules.AddAsync(rule, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(AlarmRule rule, CancellationToken ct)
    {
        _db.AlarmRules.Update(rule);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct)
    {
        var entity = await _db.AlarmRules.FindAsync(new object[] { id }, ct);
        if (entity == null)
            return;

        _db.AlarmRules.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<AlarmRule?> GetByIdAsync(long id, CancellationToken ct)
    {
        return await _db.AlarmRules
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<AlarmRule>> GetAllByDeviceIdAsync(long deviceId, CancellationToken ct)
    {
        return await _db.AlarmRules
            .Where(x => x.DeviceId == deviceId)
            .ToListAsync(ct);
    }

    public async Task<int> CountByDevice(long deviceId, CancellationToken ct)
    {
        return await _db.AlarmRules
            .Where(x => x.DeviceId == deviceId)
            .CountAsync(ct);
    }

    public async Task<bool> ExistsByIdAsync(long ruleId, CancellationToken ct)
        => await _db.AlarmRules.AnyAsync(x => x.Id == ruleId, ct);

    public async Task<bool> ExistsByUserIdAsync(long userId, long ruleId, CancellationToken ct)
        => await _db.AlarmRules.AnyAsync(x => x.Device.Aquarium.UserId == userId && x.Id == ruleId, ct);
}
