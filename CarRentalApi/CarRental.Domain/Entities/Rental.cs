using CarRental.Domain.Common;
using CarRental.Domain.Exceptions;

namespace CarRental.Domain.Entities;

public class Rental : Entity
{
    public Customer Customer { get; private set; }
    public Car Car { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTime? CancellationDate { get; private set; }

    private Rental() : base(Guid.NewGuid()) { }

    public Rental(Guid id, Customer customer, Car car, DateTime startDate, DateTime endDate, bool isCancelled = false) : base(id)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));
        if (car == null)
            throw new ArgumentNullException(nameof(car));
        if (startDate >= endDate)
            throw new DomainException("Start date must be before end date.");

        Customer = customer;
        Car = car;
        StartDate = startDate;
        EndDate = endDate;
        IsCancelled = isCancelled;
        CancellationDate = null;
    }

    public void UpdateRental(Car car, DateTime newStartDate, DateTime newEndDate)
    {
        if (newStartDate >= newEndDate)
            throw new DomainException("New start date must be before new end date.");
        if (newStartDate < DateTime.UtcNow.Date)
            throw new DomainException("New start date cannot be in the past.");
        if (car == null)
            throw new DomainException("A car needs to be specified.");

        Car = car;
        StartDate = newStartDate;
        EndDate = newEndDate;
    }

    public void Cancel()
    {
        IsCancelled = true;
        CancellationDate = DateTime.UtcNow;
    }
}