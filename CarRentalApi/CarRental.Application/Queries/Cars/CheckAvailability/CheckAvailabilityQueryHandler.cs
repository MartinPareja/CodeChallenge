using CarRental.Application.Abstractions;
using CarRental.Application.Common;
using MediatR;

namespace CarRental.Application.Queries.Cars.CheckAvailability;

public class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, PaginatedResult<CarAvailabilityDto>>
{
    private readonly ICarQueryService _carQueryService;
    private readonly ICacheService _cacheService;

    public CheckAvailabilityQueryHandler(ICarQueryService carQueryService, ICacheService cacheService)
    {
        _carQueryService = carQueryService;
        _cacheService = cacheService;
    }

    public async Task<PaginatedResult<CarAvailabilityDto>> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"CarAvailability:{request.StartDate:yyyyMMdd}:{request.EndDate:yyyyMMdd}:{request.Make}:{request.Model}:{request.Year}:{request.Location}:{request.Type}:{request.Limit}:{request.Offset}";

        var cachedResult = await _cacheService.GetAsync<PaginatedResult<CarAvailabilityDto>>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var (availableCars, totalCount) = await _carQueryService.GetAvailableCarsAsync(request);

        var result = new PaginatedResult<CarAvailabilityDto>(availableCars, request.Limit, request.Offset, totalCount);

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}