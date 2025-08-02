using CarRental.Domain.Common;

namespace CarRental.Domain.Events;

public record RentalCancelledEvent(Guid RentalId) : IDomainEvent;
