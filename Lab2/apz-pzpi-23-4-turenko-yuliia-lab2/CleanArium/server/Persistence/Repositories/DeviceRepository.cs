using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly CleanAriumDbContext _db;

    public DeviceRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task<Device?> GetByIdAsync(long id)
        => await _db.Devices.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<Device>> GetByAquariumIdAsync(long aquariumId)
        => await _db.Devices.Where(x => x.AquariumId == aquariumId).ToListAsync();

    public async Task AddAsync(Device device)
    {
        _db.Devices.Add(device);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Device device)
    {
        _db.Devices.Update(device);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Device device)
    {
        _db.Devices.Remove(device);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsByIdAsync(long deviceId, CancellationToken ct)
        => await _db.Devices.AnyAsync(x => x.Id == deviceId, ct);

    public async Task<bool> UserOwnsDeviceAsync(long userId, long deviceId, CancellationToken ct)
    {
        return await _db.Devices
            .AnyAsync(x => x.Id == deviceId && x.Aquarium.UserId == userId);
    }

    public async Task<int> CountByAquarium(long aquariumId, CancellationToken ct)
    {
        return await _db.Devices
            .Where(x => x.AquariumId == aquariumId)
            .CountAsync(ct);
    }
}