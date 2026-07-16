using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dsw2026Tpi.Data.Configurations;

public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
{
    public void Configure(EntityTypeBuilder<Availability> builder)
    {
        builder.ToTable("AVAILABILITIES");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.IsActive).HasDefaultValue(true);

        builder.Property(a => a.Month).IsRequired();
        builder.Property(a => a.Year).IsRequired();
        builder.Property(a => a.WeekDay).IsRequired();
        builder.Property(a => a.StartingHour).IsRequired();
        builder.Property(a => a.EndingHour).IsRequired();

        builder.HasOne(a => a.Doctor)
               .WithMany(d => d.Availabilities)
               .HasForeignKey(a => a.DoctorId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.DoctorId, a.Year, a.Month, a.WeekDay, a.StartingHour, a.EndingHour })
               .IsUnique();
    }
}
