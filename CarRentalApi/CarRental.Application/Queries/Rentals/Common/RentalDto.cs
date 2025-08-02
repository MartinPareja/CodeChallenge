namespace CarRental.Application.Queries.Rentals.Common;

public record RentalDto(
    Guid RentalId,
    DateTime StartDate,
    DateTime EndDate,
    bool IsCancelled,
    CarDto Car,
    CustomerDto Customer);

public record CarDto(
    Guid CarId,
    string Type,
    string Make,
    string Model,
    int Year);

public record CustomerDto(
    Guid CustomerId,
    string FullName,
    AddressDto Address);

public record AddressDto(
    string Street,
    string City,
    string CountryCode);