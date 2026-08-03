using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dsw2026Tpi.Data.Configurations;

public class AvailabilitySlotConfiguration : IEntityTypeConfiguration<AvailabilitySlot>
{
    public void Configure(EntityTypeBuilder<AvailabilitySlot> builder)
    {
        builder.ToTable("AvailabilitySlots");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Deleted).HasDefaultValue(false);
        builder.HasQueryFilter(t => !t.Deleted);

        builder.Property(t => t.Date).IsRequired();
        builder.Property(t => t.StartingTime).IsRequired();
        builder.Property(t => t.EndingTime).IsRequired();

        builder.Property(t => t.AvailabilitySlotState)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.HasOne(t => t.AvailabilityRule)
               .WithMany(a => a.AvailabilitySlots)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

