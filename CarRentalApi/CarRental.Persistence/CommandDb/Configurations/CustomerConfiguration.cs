using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.CommandDb.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.FullName).HasMaxLength(250).IsRequired();
        builder.Property(c => c.UserId).HasMaxLength(250).IsRequired();

        builder.Property(c => c.Id)
            .HasConversion(
                v => v.ToString(),
                v => Guid.Parse(v)
            );

        builder.Property(c => c.UserId)
            .HasConversion(
                v => v.ToString(),
                v => Guid.Parse(v)
            );

        builder.OwnsOne(c => c.Address, a =>
        {
            a.Property(ad => ad.Street).HasMaxLength(200).IsRequired();
            a.Property(ad => ad.City).HasMaxLength(100).IsRequired();
            a.Property(ad => ad.CountryCode).HasMaxLength(10).IsRequired();
        });
    }
}