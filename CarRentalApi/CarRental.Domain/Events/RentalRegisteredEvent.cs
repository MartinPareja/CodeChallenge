using CarRental.Domain.Common;

namespace CarRental.Domain.Events;

public record RentalRegisteredEvent(Guid RentalId, Guid CustomerId, Guid CarId, DateTime StartDate, DateTime EndDate) : IDomainEvent;
