namespace CarRental.Application.Common;

public record PaginatedResult<T>(IEnumerable<T> Data, int Limit, int Offset, int TotalCount);