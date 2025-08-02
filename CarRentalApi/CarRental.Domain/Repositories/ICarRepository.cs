using CarRental.Domain.Entities;

namespace CarRental.Domain.Repositories;

public interface ICarRepository
{
    Task<Car?> GetByIdAsync(Guid id);
    void Add(Car car);
    void Update(Car car);
    void Delete(Car car);
}