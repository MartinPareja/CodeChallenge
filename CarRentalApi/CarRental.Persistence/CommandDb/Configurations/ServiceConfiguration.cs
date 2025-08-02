using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.CommandDb.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                v => v.ToString(),
                v => Guid.Parse(v)
            );

        builder.Property(s => s.CarId).IsRequired();
        builder.Property(s => s.Date).IsRequired();

        builder.Property(c => c.CarId)
            .HasConversion(
                v => v.ToString(),
                v => Guid.Parse(v)
            );
    }
}