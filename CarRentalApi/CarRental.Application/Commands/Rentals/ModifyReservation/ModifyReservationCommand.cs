using CarRental.Application.Abstractions;

namespace CarRental.Application.Commands.Rentals.ModifyReservation;

public record ModifyReservationCommand(
    Guid RentalId,
    Guid CarId,
    DateTime NewStartDate,
    DateTime NewEndDate) : ICommand;