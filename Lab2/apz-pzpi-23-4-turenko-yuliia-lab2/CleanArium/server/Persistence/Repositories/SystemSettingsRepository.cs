using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class SystemSettingsRepository : ISystemSettingsRepository
{
    private readonly CleanAriumDbContext _db;

    public SystemSettingsRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task<SystemSettings> GetAsync(CancellationToken ct)
    {
        var settings = await _db.SystemSettings.FirstOrDefaultAsync(ct);

        if (settings == null)
        {
            settings = new SystemSettings
            {
                MaxAquariumsPerUser = 5,
                MaxDevicesPerAquarium = 10,
                MaxAlarmRulesPerDevice = 5,
                MaxScheduledCommandsPerDevice = 5
            };

            _db.SystemSettings.Add(settings);
            await _db.SaveChangesAsync(ct);
        }

        return settings;
    }

    public async Task UpdateAsync(SystemSettings settings, CancellationToken ct)
    {
        _db.SystemSettings.Update(settings);
        await _db.SaveChangesAsync(ct);
    }
}
