using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Application.Configurations;

public class AquariumConfiguration : IEntityTypeConfiguration<Aquarium>
{
    public void Configure(EntityTypeBuilder<Aquarium> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Location)
            .HasMaxLength(80);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder
            .HasMany(x => x.Devices)
            .WithOne(x => x.Aquarium)
            .HasForeignKey(x => x.AquariumId);
    }
}
