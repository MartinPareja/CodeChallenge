using CarRental.Domain.Common;

namespace CarRental.Domain.Entities;

public class Service : Entity
{
    public Guid CarId { get; private set; }
    public DateTime Date { get; private set; }

    private Service() : base(Guid.NewGuid()) { }

    public Service(Guid id, Guid carId, DateTime date) : base(id)
    {
        if (date > DateTime.UtcNow)
            throw new ArgumentException("Service date cannot be in the future.", nameof(date));

        CarId = carId;
        Date = date;
    }
}