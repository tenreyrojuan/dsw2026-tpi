using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dsw2026Tpi.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Deleted).HasDefaultValue(false);
        builder.HasQueryFilter(a => !a.Deleted);

        builder.Property(a => a.Reason).HasMaxLength(300).IsRequired();

        builder.Property(a => a.AppointmentState)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.TimeSlot)
               .WithOne(t => t.Appointment)
               .HasForeignKey<Appointment>(a => a.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.TimeSlotId).IsUnique();
    }
}
