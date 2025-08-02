using CarRental.Domain.Entities;

namespace CarRental.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    void Add(Customer customer);
    void Update(Customer customer);
    void Delete(Customer customer);
}