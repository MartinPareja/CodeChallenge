using CarRental.Application.Common;
using CarRental.Domain.Aggregates.RentalSystemAggregate;
using CarRental.Domain.Repositories;
using MediatR;

namespace CarRental.Application.Commands.Rentals.RegisterRental;

public class RegisterRentalCommandHandler : IRequestHandler<RegisterRentalCommand, RegisterRentalResult>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RentalSystemAggregateRoot _rentalSystemAggregate;

    public RegisterRentalCommandHandler(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        RentalSystemAggregateRoot rentalSystemAggregate)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _rentalSystemAggregate = rentalSystemAggregate;
    }

    public async Task<RegisterRentalResult> Handle(RegisterRentalCommand request, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetByIdAsync(request.CarId);
        if (car == null)
        {
            throw new ApplicationException($"Car with ID '{request.CarId}' not found.");
        }

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new ApplicationException($"Customer with ID '{request.CustomerId}' not found.");
        }

        var isAvailable = await _rentalRepository.IsCarAvailableAsync(request.CarId, request.StartDate, request.EndDate);
        if (!isAvailable)
        {
            throw new ApplicationException($"Car with ID '{request.CarId}' is not available for the requested period.");
        }

        var rental = _rentalSystemAggregate.RegisterRental(
            customer,
            car,
            request.StartDate,
            request.EndDate);

        _rentalRepository.Add(rental);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterRentalResult(rental.Id);
    }
}