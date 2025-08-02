using CarRental.Application.Abstractions;

namespace CarRental.Application.Commands.Rentals.CancelRental;

public record CancelRentalCommand(Guid RentalId) : ICommand;
