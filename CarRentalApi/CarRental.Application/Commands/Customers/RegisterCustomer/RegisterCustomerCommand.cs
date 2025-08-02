using CarRental.Application.Abstractions;

namespace CarRental.Application.Commands.Customers.RegisterCustomer;

public record RegisterCustomerCommand(
    string FullName,
    string Street,
    string City,
    string CountryCode,
    Guid UserId) : ICommand<RegisterCustomerResult>;

public record RegisterCustomerResult(Guid CustomerId);
