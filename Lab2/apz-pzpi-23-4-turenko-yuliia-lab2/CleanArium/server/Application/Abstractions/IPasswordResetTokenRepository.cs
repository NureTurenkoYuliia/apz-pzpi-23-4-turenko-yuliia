using Domain.Models;

namespace Application.Abstractions;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken);
    Task UpdateAsync(PasswordResetToken token, CancellationToken cancellationToken);
    Task MarkAsUsedAsync(PasswordResetToken token, CancellationToken cancellationToken);
}
