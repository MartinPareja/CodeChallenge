using CarRental.Application.Common;
using CarRental.Domain.Aggregates.RentalSystemAggregate;
using CarRental.Domain.Repositories;
using CarRental.Domain.ValueObjects;
using MediatR;

namespace CarRental.Application.Commands.Customers.RegisterCustomer;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, RegisterCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RentalSystemAggregateRoot _rentalSystemAggregate;

    public RegisterCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, RentalSystemAggregateRoot rentalSystemAggregate)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _rentalSystemAggregate = rentalSystemAggregate;
    }

    public async Task<RegisterCustomerResult> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(request.Street, request.City, request.CountryCode);

        var customer = _rentalSystemAggregate.RegisterCustomer(
            request.FullName,
            address,
            request.UserId);

        _customerRepository.Add(customer);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterCustomerResult(customer.Id);
    }
}
