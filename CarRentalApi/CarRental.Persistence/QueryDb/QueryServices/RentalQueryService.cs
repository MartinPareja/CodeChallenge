using CarRental.Application.Common;
using CarRental.Application.Queries.Rentals.Common;
using CarRental.Application.Queries.Rentals.GetRentalMetrics;
using Dapper;

namespace CarRental.Persistence.QueryDb.QueryServices;

public class RentalQueryService : IRentalQueryService
{
    private readonly DapperConnectionFactory _connectionFactory;

    public RentalQueryService(DapperConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PaginatedResult<RentalDto>> GetRentalsByUserIdAsync(Guid userId, int limit, int offset)
    {
        var strUserId = userId.ToString().ToLowerInvariant();

        const string sql = @"
            SELECT
                r.Id AS RentalId,
                r.StartDate,
                r.EndDate,
                r.IsCancelled,
                c.Id AS CustomerId,
                c.FullName,
                c.Address_Street AS Street,
                c.Address_City AS City,
                c.Address_CountryCode AS CountryCode,
                ca.Id AS CarId,
                ca.Type,
                ca.Make,
                ca.Model,
                ca.Year
            FROM
                Rentals r
            JOIN
                Customers c ON r.CustomerId = c.Id
            JOIN
                Cars ca ON r.CarId = ca.Id
            WHERE
                c.UserId = @strUserId
            ORDER BY r.StartDate DESC
            LIMIT @limit OFFSET @offset;

            SELECT COUNT(*)
            FROM
                Rentals r
            JOIN
                Customers c ON r.CustomerId = c.Id
            WHERE
                c.UserId = @strUserId;";

        using var connection = _connectionFactory.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(sql, new { strUserId, limit, offset });

        var dynamicResults = await multi.ReadAsync();
        var totalCount = await multi.ReadSingleAsync<int>();

        var parsedResults = new List<RentalDto>();

        foreach (dynamic item in dynamicResults)
        {
            try
            {
                var address = new AddressDto(
                    Street: (string)item.Street,
                    City: (string)item.City,
                    CountryCode: (string)item.CountryCode
                );

                var customer = new CustomerDto(
                    CustomerId: new Guid((string)item.CustomerId),
                    FullName: (string)item.FullName,
                    Address: address
                );

                var car = new CarDto(
                    CarId: new Guid((string)item.CarId),
                    Type: (string)item.Type,
                    Make: (string)item.Make,
                    Model: (string)item.Model,
                    Year: (int)item.Year
                );

                parsedResults.Add(new RentalDto(
                    RentalId: new Guid((string)item.RentalId),
                    StartDate: DateTime.Parse(item.StartDate),
                    EndDate: DateTime.Parse(item.EndDate),
                    IsCancelled: (long)item.IsCancelled == 1,
                    Customer: customer,
                    Car: car
                ));
            }
            catch (Exception)
            {
                continue;
            }
        }

        return new PaginatedResult<RentalDto>(parsedResults, limit, offset, totalCount);
    }

    public async Task<RentalMetricsDto> GetRentalMetricsAsync(DateTime date)
    {
        var startOfDate = date.Date;
        var endOfDate = startOfDate.AddDays(1).AddSeconds(-1);
        var sevenDaysAgo = startOfDate.AddDays(-6);

        const string sql = @"
            SELECT
                (SELECT COUNT(*) FROM Rentals r WHERE r.StartDate >= @startOfDate AND r.StartDate <= @endOfDate) AS TotalRentals,
                (SELECT COUNT(*) FROM Rentals r WHERE r.CancellationDate >= @startOfDate AND r.CancellationDate <= @endOfDate) AS TotalCancellations,
                (SELECT COUNT(DISTINCT ca.Id)
                 FROM Cars ca
                 WHERE ca.Id NOT IN (
                    SELECT r.CarId FROM Rentals r WHERE r.StartDate >= @sevenDaysAgo AND r.StartDate <= @endOfDate
                 )) AS CarsNotRented;";

        using var connection = _connectionFactory.CreateConnection();
        var dynamicResult = await connection.QuerySingleAsync<dynamic>(sql, new { startOfDate, endOfDate, sevenDaysAgo });

        var metrics = new RentalMetricsDto(
            TotalRentals: (int)dynamicResult.TotalRentals,
            TotalCancellations: (int)dynamicResult.TotalCancellations,
            CarsNotRented: (int)dynamicResult.CarsNotRented
        );

        return metrics;
    }
}