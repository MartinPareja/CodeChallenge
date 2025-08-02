using CarRental.Application.Abstractions;
using CarRental.Application.Common;

namespace CarRental.Application.Queries.Cars.CheckAvailability;

public record CheckAvailabilityQuery(
    DateTime StartDate,
    DateTime EndDate,
    string Make,
    string Model,
    int Year,
    string Location,
    string Type,
    int Limit,
    int Offset) : IQuery<PaginatedResult<CarAvailabilityDto>>;

