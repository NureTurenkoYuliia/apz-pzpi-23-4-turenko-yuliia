using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Application.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.AquariumId)
            .IsRequired();

        builder.Property(x => x.DeviceType)
            .IsRequired();

        builder.Property(x => x.DeviceStatus)
            .IsRequired();

        builder
            .HasOne(x => x.Aquarium)
            .WithMany(x => x.Devices)
            .HasForeignKey(x => x.AquariumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
