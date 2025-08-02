using CarRental.Domain.Entities;

namespace CarRental.Domain.Repositories;

public interface IRentalRepository
{
    Task<Rental?> GetByIdAsync(Guid id);
    void Add(Rental rental);
    void Update(Rental rental);
    void Delete(Rental rental);
    Task<bool> IsCarAvailableAsync(Guid carId, DateTime startDate, DateTime endDate);
}