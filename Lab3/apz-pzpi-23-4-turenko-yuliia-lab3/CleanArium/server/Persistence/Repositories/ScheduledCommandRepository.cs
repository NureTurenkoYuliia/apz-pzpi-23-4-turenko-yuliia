using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ScheduledCommandRepository : IScheduledCommandRepository
{
    private readonly CleanAriumDbContext _db;

    public ScheduledCommandRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task<ScheduledCommand?> GetByIdAsync(long id, CancellationToken ct)
    {
        return await _db.ScheduledCommands
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IEnumerable<ScheduledCommand>> GetAllByDeviceIdAsync(long deviceId)
    {
        return await _db.ScheduledCommands
            .Where(x => x.DeviceId == deviceId)
            .ToListAsync();
    }

    public async Task<ScheduledCommand> AddAsync(ScheduledCommand command)
    {
        _db.ScheduledCommands.Add(command);
        await _db.SaveChangesAsync();
        return command;
    }

    public async Task UpdateAsync(ScheduledCommand command)
    {
        _db.ScheduledCommands.Update(command);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(ScheduledCommand command)
    {
        _db.ScheduledCommands.Remove(command);
        await _db.SaveChangesAsync();
    }

    public async Task<int> CountByDevice(long deviceId, CancellationToken ct)
    {
        return await _db.ScheduledCommands
            .Where(x => x.DeviceId == deviceId)
            .CountAsync(ct);
    }

    public async Task<bool> ExistsByIdAsync(long commandId, CancellationToken ct)
        => await _db.ScheduledCommands.AnyAsync(x => x.Id == commandId, ct);

    public async Task<bool> ExistsByUserIdAsync(long userId, long commandId, CancellationToken ct)
    {
        return await _db.ScheduledCommands
            .AnyAsync(x => x.Device.Aquarium.UserId == userId && x.Id == commandId, ct);
    }
}
