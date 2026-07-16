using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dsw2026Tpi.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("DOCTORS");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.IsActive).HasDefaultValue(true);

        builder.Property(d => d.Name).HasMaxLength(50).IsRequired();

        builder.Property(d => d.LicenseNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(d => d.LicenseNumber).IsUnique();

        builder.HasOne(d => d.Speciality)
            .WithMany()
            .HasForeignKey(d => d.SpecialityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
