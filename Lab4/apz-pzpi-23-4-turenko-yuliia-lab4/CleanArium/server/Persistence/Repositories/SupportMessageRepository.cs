using Application.Abstractions;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Application;

namespace Persistence.Repositories;

public class SupportMessageRepository : ISupportMessageRepository
{
    private readonly CleanAriumDbContext _db;

    public SupportMessageRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task<SupportMessage?> GetByIdAsync(long id)
    {
        return await _db.SupportMessages
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<SupportMessage>> GetAllByUserIdAsync(long userId)
    {
        return await _db.SupportMessages
            .Where(x => x.UserId == userId && x.ReplyToMessageId == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupportMessage>> GetAllForAdminAsync()
    {
        return await _db.SupportMessages
            .Where(x => x.Sender != UserRole.Admin && x.MessageStatus == MessageStatus.Sent && x.ReplyToMessageId == null)
            .ToListAsync();
    }

    public async Task<List<SupportMessage>> GetHistoryAsync(long firstMessageId)
    {
        var result = new List<SupportMessage>();

        var root = await GetByIdAsync(firstMessageId);
        result.Add(root);

        var currentId = root.Id;

        while (true)
        {
            var next = await _db.SupportMessages
                .FirstOrDefaultAsync(x => x.ReplyToMessageId == currentId);

            if (next == null) break;

            result.Add(next);
            currentId = next.Id;
        }

        return result;
    }

    public async Task AddAsync(SupportMessage message)
    {
        _db.SupportMessages.Add(message);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(SupportMessage message)
    {
        _db.SupportMessages.Update(message);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(SupportMessage message)
    {
        _db.SupportMessages.Remove(message);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> MessageBelongsToUserAsync(long messageId, long userId)
    {
        return await _db.SupportMessages
            .AnyAsync(x => x.Id == messageId && x.UserId == userId);
    }

    public async Task<bool> ExistsAsync(long messageId, CancellationToken ct)
    {
        return await _db.SupportMessages
            .AnyAsync(x => x.Id == messageId, ct);
    }
}
