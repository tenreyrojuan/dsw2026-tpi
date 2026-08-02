using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dsw2026Tpi.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.IsActive).HasDefaultValue(true);

        builder.Property(d => d.Deleted).HasDefaultValue(false);
        builder.HasQueryFilter(d => !d.Deleted);

        builder.Property(d => d.Name).HasMaxLength(100).IsRequired();

        builder.Property(d => d.LicenseNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(d => d.LicenseNumber).IsUnique();

        builder.HasOne(d => d.Specialty)
            .WithMany()
            .HasForeignKey(d => d.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
