using Application.DTOs.Users;
using Domain.Enums;
using Domain.Models;

namespace Application.Abstractions;

public interface IUserRepository
{
    Task<List<PreviewUserDto>> GetUsersAsync(CancellationToken ct);
    Task<List<ModeratorDto>> GetModeratorsAsync(CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByIdAsync(long id, CancellationToken ct);
    Task<UserRole> GetUserRoleByIdAsync(long id, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task UpdateAsync(User user, CancellationToken ct);
    Task<bool> ExistsByIdAsync(long userId, CancellationToken ct);
    Task<List<InactiveUserDto>> GetInactiveUsersAsync(int limitDays, CancellationToken ct);
}
