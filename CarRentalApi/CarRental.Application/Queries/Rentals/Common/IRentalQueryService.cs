using CarRental.Application.Common;
using CarRental.Application.Queries.Rentals.Common;
using CarRental.Application.Queries.Rentals.GetRentalMetrics;

public interface IRentalQueryService
{
    Task<PaginatedResult<RentalDto>> GetRentalsByUserIdAsync(Guid userId, int limit, int offset);
    Task<RentalMetricsDto> GetRentalMetricsAsync(DateTime date);
}