namespace CarRental.Application.Queries.Cars.GetUpcomingServices;

using CarRental.Application.Common;
using MediatR;

public class GetUpcomingCarServicesQueryHandler : IRequestHandler<GetUpcomingCarServicesQuery, IEnumerable<CarWithServiceDto>>
{
    private readonly ICarQueryService _carQueryService;

    public GetUpcomingCarServicesQueryHandler(ICarQueryService carQueryService)
    {
        _carQueryService = carQueryService;
    }

    public async Task<IEnumerable<CarWithServiceDto>> Handle(GetUpcomingCarServicesQuery request, CancellationToken cancellationToken)
    {
        return await _carQueryService.GetUpcomingServicesAsync(request.Date);
    }
}