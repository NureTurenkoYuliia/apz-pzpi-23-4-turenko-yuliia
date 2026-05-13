using Application.Abstractions;
using Application.DTOs.Users;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Application;

namespace Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CleanAriumDbContext _db;

    public UserRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task<List<PreviewUserDto>> GetUsersAsync(CancellationToken ct)
    {
        var users = await _db.Users.Where(x => x.Role == UserRole.User).ToListAsync(ct);

        var result = new List<PreviewUserDto>();

        foreach (var user in users)
        {
            result.Add(new PreviewUserDto
            {
                UserId = user.Id,
                Email = user.Email,
                LastLoginAt = user.LastLoginAt,
            });
        }

        return result;
    }

    public async Task<List<ModeratorDto>> GetModeratorsAsync(CancellationToken ct)
    {
        var users = await _db.Users.Where(x => x.Role == UserRole.Moderator).ToListAsync(ct);

        var result = new List<ModeratorDto>();

        foreach (var user in users)
        {
            result.Add(new ModeratorDto
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            });
        }

        return result;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);

    public Task<User?> GetByIdAsync(long id, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<UserRole> GetUserRoleByIdAsync(long id, CancellationToken ct)
    {
        var role = await _db.Users
            .Where(x => x.Id == id)
            .Select(x => x.Role)
            .FirstOrDefaultAsync(ct);

        if (role == 0)
            throw new ApplicationException($"Role for user '{id}' not found in DB.");

        return role;
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByIdAsync(long userId, CancellationToken ct)
        => await _db.Users.AnyAsync(x => x.Id == userId, ct);

    public async Task<List<InactiveUserDto>> GetInactiveUsersAsync(int limitDays, CancellationToken ct)
    {
        var users = await _db.Users.Where(x => x.Role == UserRole.User)
            .Include(u => u.Aquariums)
                .ThenInclude(a => a.Devices)
            .ToListAsync(ct);

        var thresholdDate = DateTime.UtcNow.AddDays(-limitDays);

        var result = new List<InactiveUserDto>();

        foreach (var user in users)
        {
            var aquariums = user.Aquariums;
            int totalAq = aquariums.Count;
            int activeAq = aquariums.Count(a => a.IsActive);

            int activeDevices = aquariums
                .SelectMany(a => a.Devices)
                .Count(d => d.DeviceStatus == DeviceStatus.On);

            bool loginInactive = user.LastLoginAt == null || user.LastLoginAt < thresholdDate;

            bool aquariumsInactive = totalAq > 0 && activeAq == 0;
            bool devicesInactive = activeDevices == 0;

            bool userInactive =
                loginInactive &&
                aquariumsInactive &&
                devicesInactive;

            if (userInactive)
            {
                result.Add(new InactiveUserDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    LastLoginAt = user.LastLoginAt,
                    AquariumsCount = totalAq,
                    ActiveAquariums = activeAq,
                    ActiveDevices = activeDevices
                });
            }
        }

        return result;
    }
}
