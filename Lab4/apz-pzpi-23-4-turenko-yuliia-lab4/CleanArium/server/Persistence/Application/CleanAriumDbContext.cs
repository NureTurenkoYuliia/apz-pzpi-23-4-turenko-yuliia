using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Application;

public class CleanAriumDbContext(DbContextOptions<CleanAriumDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<AlarmRule> AlarmRules { get; set; }
    public DbSet<Aquarium> Aquariums { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<ExecutedCommand> ExecutedCommands { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ScheduledCommand> ScheduledCommands { get; set; }
    public DbSet<SensorData> SensorData { get; set; }
    public DbSet<SupportMessage> SupportMessages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<SystemSettings> SystemSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("CleanArium");
        builder.ApplyConfigurationsFromAssembly(typeof(CleanAriumDbContext).Assembly, t => t.Namespace == "Persistence.Application.Configurations");
    }
}
