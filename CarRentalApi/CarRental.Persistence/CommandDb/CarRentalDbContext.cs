using CarRental.Application.Common;
using CarRental.Domain.Common;
using CarRental.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Persistence.CommandDb;

public class CarRentalDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher? _publisher;

    public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : base(options) //, IPublisher publisher) : base(options)
    {
        //_publisher = publisher;
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CarRentalDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await PublishDomainEventsAsync();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEntities = ChangeTracker.Entries<AggregateRoot>()
            .Where(x => x.Entity.GetDomainEvents().Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            if (_publisher != null)
            {
                await _publisher.Publish(domainEvent);
            }
        }
    }
}