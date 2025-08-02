using CarRental.Application.Common;
using CarRental.Application.Queries.Rentals.Common;
using MediatR;

namespace CarRental.Application.Queries.Rentals.GetRentalsForUser;

public class GetRentalsForUserQueryHandler : IRequestHandler<GetRentalsForUserQuery, PaginatedResult<RentalDto>>
{
    private readonly IRentalQueryService _rentalQueryService;

    public GetRentalsForUserQueryHandler(IRentalQueryService rentalQueryService)
    {
        _rentalQueryService = rentalQueryService;
    }

    public async Task<PaginatedResult<RentalDto>> Handle(GetRentalsForUserQuery request, CancellationToken cancellationToken)
    {
        return await _rentalQueryService.GetRentalsByUserIdAsync(request.UserId, request.Limit, request.Offset);
    }
}