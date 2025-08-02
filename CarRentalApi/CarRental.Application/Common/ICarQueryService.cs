using CarRental.Application.Queries.Cars.CheckAvailability;
using CarRental.Application.Queries.Cars.GetTopRentedCars;
using CarRental.Application.Queries.Cars.GetTopRentedCarTypes;

namespace CarRental.Application.Common;

public interface ICarQueryService
{
    Task<(IEnumerable<CarAvailabilityDto> Data, int TotalCount)> GetAvailableCarsAsync(CheckAvailabilityQuery query);
    Task<IEnumerable<CarWithServiceDto>> GetUpcomingServicesAsync(DateTime date);
    Task<IEnumerable<TopRentedCarTypeDto>> GetTopRentedCarTypesAsync(DateTime startDate, DateTime endDate);
    Task<PaginatedResult<TopRentedCarDto>> GetTopRentedCarsAsync(string? make, string? model, string? type, string? location, int limit, int offset);
}