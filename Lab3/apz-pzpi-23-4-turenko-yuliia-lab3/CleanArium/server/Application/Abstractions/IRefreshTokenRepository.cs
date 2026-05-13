using Domain.Models;

namespace Application.Abstractions;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
    Task DeleteAsync(RefreshToken token);
    Task<int> DeleteExpiredAsync();
}
