using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dsw2026Tpi.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("APPOINTMENTS");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.IsActive).HasDefaultValue(true);

        builder.Property(a => a.Reason).HasMaxLength(500).IsRequired();

        builder.Property(a => a.AppointmentState)
               .HasConversion<string>()
               .HasMaxLength(30)
               .IsRequired();

        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.TimeSlot)
               .WithOne(t => t.Appointment)
               .HasForeignKey<Appointment>(a => a.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
