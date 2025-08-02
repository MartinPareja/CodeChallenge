using CarRental.Application.Abstractions;

namespace CarRental.Application.Commands.Rentals.RegisterRental;

public record RegisterRentalCommand(
    Guid CustomerId,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate) : ICommand<RegisterRentalResult>;

public record RegisterRentalResult(Guid RentalId);