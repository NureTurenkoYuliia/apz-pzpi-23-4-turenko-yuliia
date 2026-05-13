using Domain.Models;

namespace Application.Abstractions;

public interface ISupportMessageRepository
{
    Task<SupportMessage?> GetByIdAsync(long id);
    Task<IEnumerable<SupportMessage>> GetAllByUserIdAsync(long userId);
    Task<IEnumerable<SupportMessage>> GetAllForAdminAsync();
    Task<List<SupportMessage>> GetHistoryAsync(long firstMessageId);
    Task AddAsync(SupportMessage command);
    Task UpdateAsync(SupportMessage command);
    Task DeleteAsync(SupportMessage command);
    Task<bool> MessageBelongsToUserAsync(long messageId, long userId);
    Task<bool> ExistsAsync(long messageId, CancellationToken ct);
}
