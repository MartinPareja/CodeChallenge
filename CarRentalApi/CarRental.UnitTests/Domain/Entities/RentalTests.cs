using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.ValueObjects;

namespace CarRental.UnitTests.Domain.Entities;

public class RentalTests
{
    [Fact]
    public void Constructor_WithValidArguments_CreatesRentalSuccessfully()
    {
        // Arrange
        var customer = new Customer(Guid.NewGuid(), "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        var car = new Car(Guid.NewGuid(), "Toyota", "Corolla", 2020, "Sedan", "New York");
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(5);
        var rentalId = Guid.NewGuid();

        // Act
        var rental = new Rental(rentalId, customer, car, startDate, endDate);

        // Assert
        Assert.NotNull(rental);
        Assert.Equal(rentalId, rental.Id);
        Assert.Equal(customer, rental.Customer);
        Assert.Equal(car, rental.Car);
        Assert.Equal(startDate, rental.StartDate);
        Assert.Equal(endDate, rental.EndDate);
        Assert.False(rental.IsCancelled);
        Assert.Null(rental.CancellationDate);
    }

    [Fact]
    public void Constructor_WithNullCustomer_ThrowsArgumentNullException()
    {
        // Arrange
        Customer customer = null;
        var car = new Car(Guid.NewGuid(), "Toyota", "Corolla", 2020, "Sedan", "New York");
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(5);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Rental(Guid.NewGuid(), customer, car, startDate, endDate));
    }

    [Fact]
    public void Constructor_WithNullCar_ThrowsArgumentNullException()
    {
        // Arrange
        var customer = new Customer(Guid.NewGuid(), "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        Car car = null;
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(5);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Rental(Guid.NewGuid(), customer, car, startDate, endDate));
    }

    [Fact]
    public void Constructor_WithStartDateAfterEndDate_ThrowsDomainException()
    {
        // Arrange
        var customer = new Customer(Guid.NewGuid(), "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        var car = new Car(Guid.NewGuid(), "Toyota", "Corolla", 2020, "Sedan", "New York");
        var startDate = new DateTime(2023, 1, 10);
        var endDate = new DateTime(2023, 1, 5);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Rental(Guid.NewGuid(), customer, car, startDate, endDate));
        Assert.Equal("Start date must be before end date.", exception.Message);
    }

    [Fact]
    public void Constructor_WithStartDateEqualToEndDate_ThrowsDomainException()
    {
        // Arrange
        var customer = new Customer(Guid.NewGuid(), "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        var car = new Car(Guid.NewGuid(), "Toyota", "Corolla", 2020, "Sedan", "New York");
        var startDate = new DateTime(2023, 1, 5);
        var endDate = new DateTime(2023, 1, 5);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Rental(Guid.NewGuid(), customer, car, startDate, endDate));
        Assert.Equal("Start date must be before end date.", exception.Message);
    }
}