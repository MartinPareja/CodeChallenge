using CarRental.Domain.Common;

namespace CarRental.Domain.Events;

public record RentalModifiedEvent(Guid RentalId, Guid CarId, DateTime NewStartDate, DateTime NewEndDate) : IDomainEvent;
