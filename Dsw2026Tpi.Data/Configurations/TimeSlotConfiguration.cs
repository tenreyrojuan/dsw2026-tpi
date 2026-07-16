using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dsw2026Tpi.Data.Configurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TIMESLOTS");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.IsActive).HasDefaultValue(true);

        builder.Property(t => t.Date).IsRequired();
        builder.Property(t => t.StartingTime).IsRequired();
        builder.Property(t => t.EndingTime).IsRequired();

        builder.Property(t => t.TimeSlotState)
               .HasConversion<string>()
               .HasMaxLength(30)
               .IsRequired();

        builder.HasOne(t => t.Availability)
               .WithMany(a => a.TimeSlots)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

