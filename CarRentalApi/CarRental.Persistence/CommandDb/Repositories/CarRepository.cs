using CarRental.Domain.Entities;
using CarRental.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Persistence.CommandDb.Repositories;

public class CarRepository : ICarRepository
{
    private readonly CarRentalDbContext _dbContext;

    public CarRepository(CarRentalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Car?> GetByIdAsync(Guid id)
    {
        //TODO: Improve comparisson logic by removing the ToString() and reviewing compare error
        return await _dbContext.Cars.Include(c => c.Services).FirstOrDefaultAsync(c => c.Id.ToString() == id.ToString());
    }

    public void Add(Car car)
    {
        _dbContext.Cars.Add(car);
    }

    public void Update(Car car)
    {
        _dbContext.Cars.Update(car);
    }

    public void Delete(Car car)
    {
        _dbContext.Cars.Remove(car);
    }
}