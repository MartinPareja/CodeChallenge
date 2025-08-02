using CarRental.Application.Common;
using CarRental.Domain.Aggregates.RentalSystemAggregate;
using CarRental.Domain.Repositories;
using MediatR;

namespace CarRental.Application.Commands.Rentals.ModifyReservation;

public class ModifyReservationCommandHandler : IRequestHandler<ModifyReservationCommand>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RentalSystemAggregateRoot _rentalSystemAggregate;

    public ModifyReservationCommandHandler(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        IUnitOfWork unitOfWork,
        RentalSystemAggregateRoot rentalSystemAggregate)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _unitOfWork = unitOfWork;
        _rentalSystemAggregate = rentalSystemAggregate;
    }

    public async Task Handle(ModifyReservationCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId);
        if (rental == null)
        {
            throw new ApplicationException($"Rental with ID '{request.RentalId}' not found.");
        }

        var car = await _carRepository.GetByIdAsync(request.CarId);
        if (car == null)
        {
            throw new ApplicationException($"Car with ID '{request.CarId}' not found.");
        }

        if (rental.StartDate != request.NewStartDate || rental.EndDate != request.NewEndDate)
        {
            var isAvailable = await _rentalRepository.IsCarAvailableAsync(rental.Car.Id, request.NewStartDate, request.NewEndDate);
            if (!isAvailable)
            {
                throw new ApplicationException($"Car with ID '{rental.Car.Id}' is not available for the new requested period.");
            }
        }

        _rentalSystemAggregate.ModifyReservation(rental, car, request.NewStartDate, request.NewEndDate);

        _rentalRepository.Update(rental);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
