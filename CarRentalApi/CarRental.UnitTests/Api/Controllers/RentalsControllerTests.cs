using CarRental.Api.Controllers;
using CarRental.Application.Commands.Rentals.RegisterRental;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CarRental.UnitTests.Api.Controllers;

public class RentalsControllerTests
{
    private readonly Mock<ISender> _mockSender;

    private readonly RentalsController _controller;

    public RentalsControllerTests()
    {
        _mockSender = new Mock<ISender>();

        _controller = new RentalsController(_mockSender.Object);
    }

    [Fact]
    public async Task RegisterRental_ValidCommand_ReturnsCreatedAtActionResult()
    {
        var command = new RegisterRentalCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7)
        );

        var expectedResult = new RegisterRentalResult(Guid.NewGuid());

        _mockSender
            .Setup(s => s.Send(It.IsAny<RegisterRentalCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var actionResult = await _controller.RegisterRental(command);

        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);

        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);

        Assert.Equal(expectedResult, createdResult.Value);

        _mockSender.Verify(
            s => s.Send(It.Is<RegisterRentalCommand>(c => c == command), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void RegisterRental_MethodHasCorrectProducesResponseTypeAttributes()
    {
        var method = typeof(RentalsController).GetMethod(nameof(RentalsController.RegisterRental));

        var attributes = method.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false) as ProducesResponseTypeAttribute[];

        Assert.NotNull(attributes);
        Assert.Contains(attributes, attr => attr.StatusCode == StatusCodes.Status201Created);
        Assert.Contains(attributes, attr => attr.StatusCode == StatusCodes.Status400BadRequest);
    }
}