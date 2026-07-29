using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dsw2026Tpi.Data.Configurations;

public class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
{
    public void Configure(EntityTypeBuilder<Speciality> builder)
    {
        builder.ToTable("Specialities");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Deleted).HasDefaultValue(false);
        builder.HasQueryFilter(e => !e.Deleted);

        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(e => e.Name).IsUnique();

        builder.Property(e => e.Description).HasMaxLength(100);
    }
}
