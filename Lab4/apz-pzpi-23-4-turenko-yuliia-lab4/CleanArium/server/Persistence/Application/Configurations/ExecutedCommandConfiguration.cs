using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Application.Configurations;

public class ExecutedCommandConfiguration : IEntityTypeConfiguration<ExecutedCommand>
{
    public void Configure(EntityTypeBuilder<ExecutedCommand> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.DeviceId)
            .IsRequired();

        builder.Property(x => x.CommandType)
            .IsRequired();

        builder.Property(x => x.CommandStatus)
            .IsRequired();

        builder
            .HasOne(x => x.Device)
            .WithMany(x => x.ExecutedCommands)
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
