using CarRental.Application.Abstractions;
using CarRental.Application.Common;
using CarRental.Application.Queries.Rentals.Common;
using MediatR;

namespace CarRental.Application.Queries.Cars.GetTopRentedCars;

public record GetTopRentedCarsQuery(string? Make, string? Model, string? Type, string? Location, int Limit, int Offset) : IQuery<PaginatedResult<TopRentedCarDto>>;

public record TopRentedCarDto(CarDto Car, int RentalCount);

public class GetTopRentedCarsQueryHandler : IRequestHandler<GetTopRentedCarsQuery, PaginatedResult<TopRentedCarDto>>
{
    private readonly ICarQueryService _carQueryService;

    public GetTopRentedCarsQueryHandler(ICarQueryService carQueryService)
    {
        _carQueryService = carQueryService;
    }

    public async Task<PaginatedResult<TopRentedCarDto>> Handle(GetTopRentedCarsQuery request, CancellationToken cancellationToken)
    {
        return await _carQueryService.GetTopRentedCarsAsync(request.Make, request.Model, request.Type, request.Location, request.Limit, request.Offset);
    }
}