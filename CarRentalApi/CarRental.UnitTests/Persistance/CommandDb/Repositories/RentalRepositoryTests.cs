using CarRental.Domain.Entities;
using CarRental.Domain.ValueObjects;
using CarRental.Persistence.CommandDb;
using CarRental.Persistence.CommandDb.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarRental.UnitTests.Persistence.CommandDb.Repositories;

public class RentalRepositoryTests
{
    private readonly DbContextOptions<CarRentalDbContext> _dbContextOptions;

    public RentalRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private CarRentalDbContext CreateDbContext()
    {
        var dbContext = new CarRentalDbContext(_dbContextOptions);
        dbContext.Database.EnsureCreated();
        return dbContext;
    }

    [Fact]
    public async Task IsCarAvailableAsync_WhenNoOverlappingRentals_ReturnsTrue()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var car = new Car(carId, "Toyota", "Corolla", 2020, "Sedan", "New York");
        var customer = new Customer(customerId, "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        var existingRental = new Rental(Guid.NewGuid(), customer, car, new DateTime(2023, 1, 1), new DateTime(2023, 1, 5), false);
        var newRentalStartDate = new DateTime(2023, 1, 6);
        var newRentalEndDate = new DateTime(2023, 1, 10);

        using (var context = CreateDbContext())
        {
            // The Car and Customer are added implicitly when the Rental is added.
            context.Rentals.Add(existingRental);
            await context.SaveChangesAsync();

            var repository = new RentalRepository(context);

            // Act
            var isAvailable = await repository.IsCarAvailableAsync(carId, newRentalStartDate, newRentalEndDate);

            // Assert
            Assert.True(isAvailable);
        }
    }

    [Fact]
    public async Task IsCarAvailableAsync_WhenExactOverlappingRentals_ReturnsFalse()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var car = new Car(carId, "Toyota", "Corolla", 2020, "Sedan", "New York");
        var customer = new Customer(customerId, "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        var existingRental = new Rental(Guid.NewGuid(), customer, car, new DateTime(2023, 1, 1), new DateTime(2023, 1, 5), false);
        var newRentalStartDate = new DateTime(2023, 1, 1);
        var newRentalEndDate = new DateTime(2023, 1, 5);

        using (var context = CreateDbContext())
        {
            context.Rentals.Add(existingRental);
            await context.SaveChangesAsync();

            var repository = new RentalRepository(context);

            // Act
            var isAvailable = await repository.IsCarAvailableAsync(carId, newRentalStartDate, newRentalEndDate);

            // Assert
            Assert.False(isAvailable);
        }
    }

    [Fact]
    public async Task IsCarAvailableAsync_WhenPartialOverlappingRentals_ReturnsFalse()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var car = new Car(carId, "Toyota", "Corolla", 2020, "Sedan", "New York");
        var customer = new Customer(customerId, "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        var existingRental = new Rental(Guid.NewGuid(), customer, car, new DateTime(2023, 1, 1), new DateTime(2023, 1, 5), false);
        var newRentalStartDate = new DateTime(2023, 1, 3); // Starts within the existing rental
        var newRentalEndDate = new DateTime(2023, 1, 7);

        using (var context = CreateDbContext())
        {
            context.Rentals.Add(existingRental);
            await context.SaveChangesAsync();

            var repository = new RentalRepository(context);

            // Act
            var isAvailable = await repository.IsCarAvailableAsync(carId, newRentalStartDate, newRentalEndDate);

            // Assert
            Assert.False(isAvailable);
        }
    }

    [Fact]
    public async Task IsCarAvailableAsync_WhenNewRentalContainsExistingRental_ReturnsFalse()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var car = new Car(carId, "Toyota", "Corolla", 2020, "Sedan", "New York");
        var customer = new Customer(customerId, "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        var existingRental = new Rental(Guid.NewGuid(), customer, car, new DateTime(2023, 1, 3), new DateTime(2023, 1, 7), false);
        var newRentalStartDate = new DateTime(2023, 1, 1);
        var newRentalEndDate = new DateTime(2023, 1, 10);

        using (var context = CreateDbContext())
        {
            context.Rentals.Add(existingRental);
            await context.SaveChangesAsync();

            var repository = new RentalRepository(context);

            // Act
            var isAvailable = await repository.IsCarAvailableAsync(carId, newRentalStartDate, newRentalEndDate);

            // Assert
            Assert.False(isAvailable);
        }
    }

    [Fact]
    public async Task Add_AddsNewRentalToDbContext()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var car = new Car(carId, "Toyota", "Corolla", 2020, "Sedan", "New York");
        var customer = new Customer(customerId, "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());
        var newRental = new Rental(Guid.NewGuid(), customer, car, new DateTime(2023, 1, 1), new DateTime(2023, 1, 5), false);

        using (var context = CreateDbContext())
        {
            var repository = new RentalRepository(context);

            // Act
            repository.Add(newRental);
            await context.SaveChangesAsync();

            // Assert
            var addedRental = await context.Rentals.FindAsync(newRental.Id);
            Assert.NotNull(addedRental);
            Assert.Equal(newRental.Id, addedRental.Id);
        }
    }
}