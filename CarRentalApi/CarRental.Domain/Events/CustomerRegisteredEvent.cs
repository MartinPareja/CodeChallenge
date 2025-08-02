using CarRental.Domain.Common;

namespace CarRental.Domain.Events;

public record CustomerRegisteredEvent(Guid CustomerId, string FullName, Guid userId) : IDomainEvent;