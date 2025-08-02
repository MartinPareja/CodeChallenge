using CarRental.Domain.Entities;
using CarRental.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Persistence.CommandDb.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly CarRentalDbContext _dbContext;

    public RentalRepository(CarRentalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public void Add(Rental rental)
    {
        _dbContext.Rentals.Add(rental);
    }

    public void Update(Rental rental)
    {
        _dbContext.Rentals.Update(rental);
    }

    public void Delete(Rental rental)
    {
        _dbContext.Rentals.Remove(rental);
    }

    public async Task<bool> IsCarAvailableAsync(Guid carId, DateTime startDate, DateTime endDate)
    {
        var overlappingRentals = await _dbContext.Rentals
            .AnyAsync(r => r.Car.Id == carId &&
                           ((startDate < r.EndDate && endDate > r.StartDate) ||
                            (startDate == r.StartDate && endDate == r.EndDate)));

        return !overlappingRentals;
    }
}