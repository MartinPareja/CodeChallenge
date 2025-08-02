using CarRental.Application.Abstractions;
using CarRental.Application.Common;
using CarRental.Application.Queries.Cars.CheckAvailability;
using Moq;

namespace CarRental.UnitTests.Application.Queries.Cars.CheckAvailability;

public class CheckAvailabilityQueryHandlerTests
{
    private readonly Mock<ICarQueryService> _mockCarQueryService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly CheckAvailabilityQueryHandler _handler;

    public CheckAvailabilityQueryHandlerTests()
    {
        _mockCarQueryService = new Mock<ICarQueryService>();
        _mockCacheService = new Mock<ICacheService>();
        _handler = new CheckAvailabilityQueryHandler(_mockCarQueryService.Object, _mockCacheService.Object);
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ReturnsCachedResultAndDoesNotCallService()
    {
        // Arrange
        var query = new CheckAvailabilityQuery(
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7),
            "Toyota", "Corolla", 2020, "New York", "Sedan", 0, 10);

        var expectedResult = new PaginatedResult<CarAvailabilityDto>(
            new List<CarAvailabilityDto> { new CarAvailabilityDto(Guid.NewGuid(), "Toyota", "Corolla", 2020, "New York", true) },
            10, 0, 1);

        _mockCacheService.Setup(c => c.GetAsync<PaginatedResult<CarAvailabilityDto>>(It.IsAny<string>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
        _mockCarQueryService.Verify(s => s.GetAvailableCarsAsync(It.IsAny<CheckAvailabilityQuery>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_CallsServiceAndCachesResult()
    {
        // Arrange
        var query = new CheckAvailabilityQuery(
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7),
            "Honda", "Civic", 2021, "Los Angeles", "Sedan", 0, 10);

        _mockCacheService.Setup(c => c.GetAsync<PaginatedResult<CarAvailabilityDto>>(It.IsAny<string>()))
            .ReturnsAsync((PaginatedResult<CarAvailabilityDto>?)null);

        var carsFromService = new List<CarAvailabilityDto>
        {
            new CarAvailabilityDto(Guid.NewGuid(), "Honda", "Civic", 2021, "Los Angeles", true)
        };
        _mockCarQueryService.Setup(s => s.GetAvailableCarsAsync(It.IsAny<CheckAvailabilityQuery>()))
            .ReturnsAsync((carsFromService, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockCarQueryService.Verify(s => s.GetAvailableCarsAsync(It.IsAny<CheckAvailabilityQuery>()), Times.Once);
        _mockCacheService.Verify(c => c.SetAsync(
            It.IsAny<string>(),
            It.Is<PaginatedResult<CarAvailabilityDto>>(p => p.Data.Count() == carsFromService.Count),
            It.IsAny<TimeSpan?>()), Times.Once);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(query.Limit, result.Limit);
        Assert.Equal(query.Offset, result.Offset);
    }
}