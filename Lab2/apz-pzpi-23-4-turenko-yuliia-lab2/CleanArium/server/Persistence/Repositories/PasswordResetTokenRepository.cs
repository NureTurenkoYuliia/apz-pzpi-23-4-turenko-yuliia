using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly CleanAriumDbContext _db;

    public PasswordResetTokenRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _db.PasswordResetTokens
            .FirstOrDefaultAsync(x => x.Token == token && !x.IsUsed, cancellationToken);
    }

    public async Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
    {
        await _db.PasswordResetTokens.AddAsync(token, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
    {
        _db.PasswordResetTokens.Update(token);
        await _db.SaveChangesAsync();
    }

    public async Task MarkAsUsedAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
    {
        token.IsUsed = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
