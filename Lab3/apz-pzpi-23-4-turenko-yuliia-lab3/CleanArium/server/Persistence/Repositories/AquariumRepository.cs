using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class AquariumRepository : IAquariumRepository
{
    private readonly CleanAriumDbContext _db;

    public AquariumRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task<Aquarium?> GetByIdAsync(long id)
        => await _db.Aquariums.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<Aquarium>> GetAllByUserIdAsync(long userId)
        => await _db.Aquariums.Where(x => x.UserId == userId).ToListAsync();

    public async Task<List<Aquarium>> GetAllWithDevicesByUserIdAsync(long userId)
        => await _db.Aquariums.Where(x => x.UserId == userId).Include(x => x.Devices).ToListAsync();

    public async Task AddAsync(Aquarium aquarium)
    {
        await _db.Aquariums.AddAsync(aquarium);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Aquarium aquarium)
    {
        _db.Aquariums.Update(aquarium);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Aquarium aquarium)
    {
        _db.Aquariums.Remove(aquarium);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsByUserIdAsync(long userId, long aquariumId, CancellationToken ct)
        => await _db.Aquariums.AnyAsync(x => x.UserId == userId && x.Id == aquariumId, ct);

    public async Task<bool> ExistsByNameAsync(long userId, string name, CancellationToken ct)
        => await _db.Aquariums.AnyAsync(x => x.UserId == userId && x.Name == name, ct);

    public async Task<int> CountByUser(long userId, CancellationToken ct)
    {
        return await _db.Aquariums
            .Where(x => x.UserId == userId)
            .CountAsync(ct);
    }
}
