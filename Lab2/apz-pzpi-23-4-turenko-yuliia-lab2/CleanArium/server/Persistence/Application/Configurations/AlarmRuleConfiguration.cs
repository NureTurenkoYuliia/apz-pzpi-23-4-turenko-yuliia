using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Application.Configurations;

public class AlarmRuleConfiguration : IEntityTypeConfiguration<AlarmRule>
{
    public void Configure(EntityTypeBuilder<AlarmRule> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.DeviceId)
            .IsRequired();

        builder.Property(x => x.Condition)
            .IsRequired();

        builder
            .HasOne(x => x.Device)
            .WithMany(x => x.AlarmRules)
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
