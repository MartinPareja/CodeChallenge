using CarRental.Application.Commands.Rentals.RegisterRental;
using CarRental.Application.Common;
using CarRental.Domain.Aggregates.RentalSystemAggregate;
using CarRental.Domain.Entities;
using CarRental.Domain.Repositories;
using CarRental.Domain.ValueObjects;
using Moq;

namespace CarRental.UnitTests.Application.Commands.Rentals.RegisterRental;

public class RegisterRentalCommandHandlerTests
{
    private readonly Mock<IRentalRepository> _mockRentalRepository;
    private readonly Mock<ICarRepository> _mockCarRepository;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly RentalSystemAggregateRoot _rentalSystemAggregate;
    private readonly RegisterRentalCommandHandler _handler;

    public RegisterRentalCommandHandlerTests()
    {
        _mockRentalRepository = new Mock<IRentalRepository>();
        _mockCarRepository = new Mock<ICarRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _rentalSystemAggregate = new RentalSystemAggregateRoot();

        _handler = new RegisterRentalCommandHandler(
            _mockRentalRepository.Object,
            _mockCarRepository.Object,
            _mockCustomerRepository.Object,
            _mockUnitOfWork.Object,
            _rentalSystemAggregate);
    }

    [Fact]
    public async Task Handle_ValidRequest_RegistersRentalAndSavesChanges()
    {
        var carId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(3);

        var request = new RegisterRentalCommand(customerId, carId, startDate, endDate);
        var car = new Car(carId, "Toyota", "Corolla", 2020, "Sedan", "New York");
        var customer = new Customer(customerId, "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());

        _mockCarRepository.Setup(r => r.GetByIdAsync(carId)).ReturnsAsync(car);
        _mockCustomerRepository.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync(customer);
        _mockRentalRepository.Setup(r => r.IsCarAvailableAsync(carId, startDate, endDate)).ReturnsAsync(true);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.RentalId);
        _mockRentalRepository.Verify(r => r.Add(It.IsAny<Rental>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CarNotFound_ThrowsApplicationException()
    {
        var request = new RegisterRentalCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
        _mockCarRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Car)null);

        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal($"Car with ID '{request.CarId}' not found.", exception.Message);
        _mockRentalRepository.Verify(r => r.Add(It.IsAny<Rental>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ThrowsApplicationException()
    {
        var carId = Guid.NewGuid();
        var request = new RegisterRentalCommand(Guid.NewGuid(), carId, DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
        _mockCarRepository.Setup(r => r.GetByIdAsync(carId)).ReturnsAsync(new Car(carId, "Toyota", "Corolla", 2020, "Sedan", "New York"));
        _mockCustomerRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Customer)null);

        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal($"Customer with ID '{request.CustomerId}' not found.", exception.Message);
        _mockRentalRepository.Verify(r => r.Add(It.IsAny<Rental>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CarNotAvailable_ThrowsApplicationException()
    {
        var carId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(3);

        var request = new RegisterRentalCommand(customerId, carId, startDate, endDate);
        var car = new Car(carId, "Toyota", "Corolla", 2020, "Sedan", "New York");
        var customer = new Customer(customerId, "John Doe", new Address("123 Main St", "Anytown", "USA"), Guid.NewGuid());

        _mockCarRepository.Setup(r => r.GetByIdAsync(carId)).ReturnsAsync(car);
        _mockCustomerRepository.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync(customer);
        _mockRentalRepository.Setup(r => r.IsCarAvailableAsync(carId, startDate, endDate)).ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal($"Car with ID '{request.CarId}' is not available for the requested period.", exception.Message);
        _mockRentalRepository.Verify(r => r.Add(It.IsAny<Rental>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}