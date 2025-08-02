namespace CarRental.Application.Queries.Cars.GetUpcomingServices;

using CarRental.Application.Common;
using MediatR;

public record GetUpcomingCarServicesQuery(DateTime Date) : IRequest<IEnumerable<CarWithServiceDto>>;