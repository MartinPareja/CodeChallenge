using CarRental.Domain.Common;
using CarRental.Domain.ValueObjects;

namespace CarRental.Domain.Entities;

public class Customer : Entity
{
    public string FullName { get; private set; }
    public Address Address { get; private set; }
    public Guid UserId { get; private set; }

    private Customer() : base(Guid.NewGuid()) { }

    public Customer(Guid id, string fullName, Address address, Guid userId) : base(id)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));
        if (address == null)
            throw new ArgumentNullException(nameof(address));
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        FullName = fullName;
        Address = address;
        UserId = userId;
    }

    public void UpdateDetails(string newFullName, Address newAddress)
    {
        FullName = newFullName;
        Address = newAddress;
    }
}