using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.CommandDb.Configurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                v => v.ToString(),
                v => Guid.Parse(v)
            );

        builder.HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey("CustomerId")
            .IsRequired();

        builder.HasOne(r => r.Car)
            .WithMany()
            .HasForeignKey("CarId")
            .IsRequired();

        builder.Property(r => r.StartDate).IsRequired();
        builder.Property(r => r.EndDate).IsRequired();
        builder.Property(r => r.IsCancelled)
            .IsRequired();
        builder.Property(r => r.CancellationDate)
            .IsRequired(false);
    }
}