using CarRental.Application.Abstractions;
using CarRental.Application.Common;
using MediatR;

namespace CarRental.Application.Queries.Cars.GetTopRentedCarTypes;

public record GetTopRentedCarTypesQuery(DateTime StartDate, DateTime EndDate) : IQuery<IEnumerable<TopRentedCarTypeDto>>;

public record TopRentedCarTypeDto(string CarType, decimal UtilizationPercentage);

public class GetTopRentedCarTypesQueryHandler : IRequestHandler<GetTopRentedCarTypesQuery, IEnumerable<TopRentedCarTypeDto>>
{
    private readonly ICarQueryService _carQueryService;

    public GetTopRentedCarTypesQueryHandler(ICarQueryService carQueryService)
    {
        _carQueryService = carQueryService;
    }

    public async Task<IEnumerable<TopRentedCarTypeDto>> Handle(GetTopRentedCarTypesQuery request, CancellationToken cancellationToken)
    {
        return await _carQueryService.GetTopRentedCarTypesAsync(request.StartDate, request.EndDate);
    }
}