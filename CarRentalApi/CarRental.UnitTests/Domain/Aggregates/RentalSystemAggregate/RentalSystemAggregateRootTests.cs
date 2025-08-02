using CarRental.Domain.Aggregates.RentalSystemAggregate;
using CarRental.Domain.Entities;
using CarRental.Domain.Events;
using CarRental.Domain.ValueObjects;

namespace CarRental.UnitTests.Domain.Aggregates.RentalSystemAggregate;

public class RentalSystemAggregateRootTests
{
    [Fact]
    public void RegisterRental_WithValidArguments_ReturnsRentalAndAddsEvent()
    {
        // Arrange
        var aggregate = new RentalSystemAggregateRoot();
        var customer = new Customer(Guid.NewGuid(), "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        var car = new Car(Guid.NewGuid(), "Toyota", "Corolla", 2020, "Sedan", "New York");
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = startDate.AddDays(5);

        // Act
        var rental = aggregate.RegisterRental(customer, car, startDate, endDate);

        // Assert
        // Check that a rental object was created and its properties are correct.
        Assert.NotNull(rental);
        Assert.Equal(customer, rental.Customer);
        Assert.Equal(car, rental.Car);
        Assert.Equal(startDate, rental.StartDate);
        Assert.Equal(endDate, rental.EndDate);

        // Check that a single domain event was added.
        var domainEvent = Assert.Single(aggregate.GetDomainEvents());

        // Check that the domain event is of the correct type and has the correct properties.
        var rentalRegisteredEvent = Assert.IsType<RentalRegisteredEvent>(domainEvent);
        Assert.Equal(rental.Id, rentalRegisteredEvent.RentalId);
        Assert.Equal(customer.Id, rentalRegisteredEvent.CustomerId);
        Assert.Equal(car.Id, rentalRegisteredEvent.CarId);
        Assert.Equal(startDate, rentalRegisteredEvent.StartDate);
        Assert.Equal(endDate, rentalRegisteredEvent.EndDate);
    }
}