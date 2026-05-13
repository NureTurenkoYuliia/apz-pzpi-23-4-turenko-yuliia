using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly CleanAriumDbContext _db;

    public RefreshTokenRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task AddAsync(RefreshToken token)
    {
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _db.RefreshTokens.Update(token);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(RefreshToken token)
    {
        _db.RefreshTokens.Remove(token);
        await _db.SaveChangesAsync();
    }

    public async Task<int> DeleteExpiredAsync()
    {
        var expiredTokens = await _db.RefreshTokens
            .Where(x => x.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        if (expiredTokens.Count == 0)
            return 0;

        _db.RefreshTokens.RemoveRange(expiredTokens);
        return await _db.SaveChangesAsync();
    }
}
