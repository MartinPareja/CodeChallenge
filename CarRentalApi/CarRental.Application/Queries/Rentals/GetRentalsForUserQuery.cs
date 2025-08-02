using CarRental.Application.Abstractions;
using CarRental.Application.Common;
using CarRental.Application.Queries.Rentals.Common;

namespace CarRental.Application.Queries.Rentals.GetRentalsForUser;

public record GetRentalsForUserQuery(Guid UserId, int Limit, int Offset) : IQuery<PaginatedResult<RentalDto>>;