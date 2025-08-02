using CarRental.Domain.Common;
using CarRental.Domain.Entities;
using CarRental.Domain.Events;
using CarRental.Domain.ValueObjects;

namespace CarRental.Domain.Aggregates.RentalSystemAggregate;

public class RentalSystemAggregateRoot : AggregateRoot
{
    public RentalSystemAggregateRoot() : base() { }

    public Customer RegisterCustomer(string fullName, Address address, Guid userId)
    {
        var customer = new Customer(Guid.NewGuid(), fullName, address, userId);

        AddDomainEvent(new CustomerRegisteredEvent(customer.Id, customer.FullName, userId));

        return customer;
    }

    public Rental RegisterRental(Customer customer, Car car, DateTime startDate, DateTime endDate)
    {
        var rental = new Rental(Guid.NewGuid(), customer, car, startDate, endDate);

        AddDomainEvent(new RentalRegisteredEvent(rental.Id, customer.Id, car.Id, startDate, endDate));

        return rental;
    }

    public void ModifyReservation(Rental rental, Car car, DateTime newStartDate, DateTime newEndDate)
    {
        rental.UpdateRental(car, newStartDate, newEndDate);

        AddDomainEvent(new RentalModifiedEvent(rental.Id, car.Id, newStartDate, newEndDate));
    }

    public void CancelRental(Rental rental)
    {
        rental.Cancel();

        AddDomainEvent(new RentalCancelledEvent(rental.Id));
    }
}