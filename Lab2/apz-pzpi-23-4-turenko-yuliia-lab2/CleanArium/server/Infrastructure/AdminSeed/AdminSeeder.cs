using Application.Abstractions;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.AdminSeed;

public class AdminSeeder
{
    private readonly CleanAriumDbContext _dbContext;
    private readonly ILogger<AdminSeeder> _logger;
    private readonly IPasswordHasher _passwordHasher;

    public AdminSeeder(
        CleanAriumDbContext dbContext,
        ILogger<AdminSeeder> logger,
        IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Checking if admin user exists...");

        var existingAdmin = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Role == UserRole.Admin);

        if (existingAdmin != null)
        {
            _logger.LogInformation("Admin already exists. Skipping seeding.");
            return;
        }

        _logger.LogInformation("Admin does not exist. Creating default admin...");

        var salt = Guid.NewGuid().ToString("N");

        var admin = new User
        {
            UserName = "admin",
            Email = "admin@example.com",
            Role = UserRole.Admin,
            Salt = salt,
            PasswordHash = _passwordHasher.HashPassword("Admin123!", salt),
            CreatedAt = DateTime.UtcNow,
            IsBlocked = false
        };

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Admin user created successfully.");
    }
}
