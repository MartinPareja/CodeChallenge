using CarRental.Application.Abstractions;
using MediatR;

namespace CarRental.Application.Queries.Rentals.GetRentalMetrics;

public record GetRentalMetricsQuery(DateTime Date) : IQuery<RentalMetricsDto>;

public record RentalMetricsDto(int TotalRentals, int TotalCancellations, int CarsNotRented);

public class GetRentalMetricsQueryHandler : IRequestHandler<GetRentalMetricsQuery, RentalMetricsDto>
{
    private readonly IRentalQueryService _rentalQueryService;

    public GetRentalMetricsQueryHandler(IRentalQueryService rentalQueryService)
    {
        _rentalQueryService = rentalQueryService;
    }

    public async Task<RentalMetricsDto> Handle(GetRentalMetricsQuery request, CancellationToken cancellationToken)
    {
        return await _rentalQueryService.GetRentalMetricsAsync(request.Date);
    }
}