using CarRental.Domain.Common;

namespace CarRental.Domain.Entities;

public class Car : Entity
{
    public string Make { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public string Type { get; private set; }
    public string Location { get; private set; }
    public ICollection<Service> Services { get; private set; } = new List<Service>();

    private Car() : base(Guid.NewGuid()) { }

    public Car(Guid id, string make, string model, int year, string type, string location) : base(id)
    {
        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("Make cannot be empty.", nameof(make));
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty.", nameof(model));
        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Invalid year.", nameof(year));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type cannot be empty.", nameof(type));
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));

        Make = make;
        Model = model;
        Year = year;
        Type = type;
        Location = location;
    }

    public void AddService(Service service)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));
        Services.Add(service);
    }
}