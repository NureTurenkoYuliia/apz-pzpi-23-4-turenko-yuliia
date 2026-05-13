using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Application.Configurations;

public class ScheduledCommandConfiguration : IEntityTypeConfiguration<ScheduledCommand>
{
    public void Configure(EntityTypeBuilder<ScheduledCommand> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.DeviceId)
            .IsRequired();

        builder.Property(x => x.CommandType)
            .IsRequired();

        builder.Property(x => x.RepeatMode)
            .IsRequired();

        builder
            .HasOne(x => x.Device)
            .WithMany(x => x.ScheduledCommands)
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
