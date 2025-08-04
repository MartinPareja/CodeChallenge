using CarRental.Application.Common;
using CarRental.Domain.Aggregates.RentalSystemAggregate;
using CarRental.Domain.Repositories;
using MediatR;

namespace CarRental.Application.Commands.Rentals.CancelRental;

public class CancelRentalCommandHandler : IRequestHandler<CancelRentalCommand>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RentalSystemAggregateRoot _rentalSystemAggregate;

    public CancelRentalCommandHandler(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        IUnitOfWork unitOfWork,
        RentalSystemAggregateRoot rentalSystemAggregate)
    {
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
        _rentalSystemAggregate = rentalSystemAggregate;
    }

    public async Task Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId);
        if (rental == null)
        {
            throw new ApplicationException($"Rental with ID '{request.RentalId}' not found.");
        }

        _rentalSystemAggregate.CancelRental(rental);

        _rentalRepository.Update(rental);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
