using CarRental.Domain.Entities;
using CarRental.Domain.Repositories;

namespace CarRental.Persistence.CommandDb.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CarRentalDbContext _dbContext;

    public CustomerRepository(CarRentalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Customers.FindAsync(id);
    }

    public void Add(Customer customer)
    {
        _dbContext.Customers.Add(customer);
    }

    public void Update(Customer customer)
    {
        _dbContext.Customers.Update(customer);
    }

    public void Delete(Customer customer)
    {
        _dbContext.Customers.Remove(customer);
    }
}