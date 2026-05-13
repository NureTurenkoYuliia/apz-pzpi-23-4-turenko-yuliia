using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ExecutedCommandRepository : IExecutedCommandRepository
{
    private readonly CleanAriumDbContext _db;

    public ExecutedCommandRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(ExecutedCommand command)
    {
        _db.ExecutedCommands.Add(command);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<ExecutedCommand>> GetAllByDeviceIdAsync(long deviceId)
    {
        return await _db.ExecutedCommands
            .AsNoTracking()
            .Where(c => c.DeviceId == deviceId)
            .OrderBy(c => c.IssuedAt)
            .ToListAsync();
    }

    public async Task<List<ExecutedCommand>> GetByPeriodAsync(DateTime from, DateTime to)
    {
        return await _db.ExecutedCommands.Where(c => c.IssuedAt >= from && c.IssuedAt <= to)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> CountByDeviceAsync( long deviceId, DateTime from, DateTime to)
    {
        return await _db.ExecutedCommands
            .Where(c => c.DeviceId == deviceId && c.IssuedAt >= from && c.IssuedAt <= to)
            .CountAsync();
    }
}
