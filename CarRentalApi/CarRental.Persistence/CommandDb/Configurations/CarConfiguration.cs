using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.CommandDb.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Make).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Model).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Year).IsRequired();
        builder.Property(c => c.Type).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Location).HasMaxLength(100).IsRequired();

        builder.Property(c => c.Id)
            .HasConversion(                
                v => v.ToString(),
                v => Guid.Parse(v) 
            );

        builder.HasMany(c => c.Services)
            .WithOne()
            .HasForeignKey(s => s.CarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}