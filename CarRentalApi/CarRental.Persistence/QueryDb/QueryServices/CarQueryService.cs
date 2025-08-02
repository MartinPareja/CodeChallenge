using CarRental.Application.Common;
using CarRental.Application.Queries.Cars.CheckAvailability;
using CarRental.Application.Queries.Cars.GetTopRentedCars;
using CarRental.Application.Queries.Cars.GetTopRentedCarTypes;
using CarRental.Application.Queries.Rentals.Common;
using CarRental.Domain.Entities;
using Dapper;
using System.Globalization;

namespace CarRental.Persistence.QueryDb.QueryServices;

public class CarQueryService : ICarQueryService
{
    private readonly DapperConnectionFactory _connectionFactory;

    public CarQueryService(DapperConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<(IEnumerable<CarAvailabilityDto> Data, int TotalCount)> GetAvailableCarsAsync(CheckAvailabilityQuery query)
    {
        const string countSql = @"
            SELECT
                COUNT(c.Id)
            FROM Cars c
            WHERE
                (@Make IS NULL OR @Make = '' OR c.Make LIKE '%' || @Make || '%')
                AND (@Model IS NULL OR @Model = '' OR c.Model LIKE '%' || @Model || '%')
                AND (@Year IS NULL OR c.Year = @Year)
                AND (@Location IS NULL OR @Location = '' OR c.Location LIKE '%' || @Location || '%')
                AND (@Type IS NULL OR @Type = '' OR c.Type LIKE '%' || @Type || '%');
        ";

        const string dataSql = @"
            SELECT
                c.Id AS CarId,
                c.Make,
                c.Model,
                CAST(c.Year AS INTEGER) AS Year,
                c.Location,
                CAST(
                    CASE
                        WHEN EXISTS (
                            SELECT 1
                            FROM Rentals r
                            WHERE r.CarId = c.Id
                              AND r.IsCancelled = 0
                              AND (
                                    datetime(@StartDate) < datetime(r.EndDate) AND datetime(@EndDate) > datetime(r.StartDate)
                                )
                        ) THEN 0
                        WHEN EXISTS (
                            SELECT 1
                            FROM Rentals r
                            WHERE r.CarId = c.Id
                              AND r.IsCancelled = 0
                              AND datetime(@StartDate) < datetime(r.EndDate, '+1 day')
                              AND datetime(r.EndDate) <= datetime(@StartDate)
                        ) THEN 0
                        WHEN EXISTS (
                            SELECT 1
                            FROM Services s
                            WHERE s.CarId = c.Id
                              AND (
                                    datetime(@StartDate) < datetime(s.Date, '+2 days') AND datetime(@EndDate) > datetime(s.Date)
                                )
                        ) THEN 0
                        ELSE 1
                    END
                AS INTEGER) AS IsAvailable
            FROM Cars c
            WHERE
                (@Make IS NULL OR @Make = '' OR c.Make LIKE '%' || @Make || '%')
                AND (@Model IS NULL OR @Model = '' OR c.Model LIKE '%' || @Model || '%')
                AND (@Year IS NULL OR c.Year = @Year)
                AND (@Location IS NULL OR @Location = '' OR c.Location LIKE '%' || @Location || '%')
                AND (@Type IS NULL OR @Type = '' OR c.Type LIKE '%' || @Type || '%')
            LIMIT @Limit OFFSET @Offset;
        ";

        var parameters = new
        {
            query.StartDate,
            query.EndDate,
            Make = string.IsNullOrWhiteSpace(query.Make) ? null : query.Make,
            Model = string.IsNullOrWhiteSpace(query.Model) ? null : query.Model,
            Year = query.Year == 0 ? (int?)null : query.Year,
            Location = string.IsNullOrWhiteSpace(query.Location) ? null : query.Location,
            Type = string.IsNullOrWhiteSpace(query.Type) ? null : query.Type,
            query.Limit,
            query.Offset
        };

        using var connection = _connectionFactory.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var dynamicResults = await connection.QueryAsync(dataSql, parameters);

        var parsedResults = new List<CarAvailabilityDto>();

        foreach (dynamic item in dynamicResults)
        {
            parsedResults.Add(new CarAvailabilityDto(
                CarId: item.CarId is string carIdString ? new Guid(carIdString) : (Guid)item.CarId,
                Make: (string)item.Make,
                Model: (string)item.Model,
                Year: (int)item.Year,
                Location: (string)item.Location,
                IsAvailable: Convert.ToBoolean((int)item.IsAvailable)
            ));
        }

        return (parsedResults, totalCount);
    }

    public async Task<IEnumerable<CarWithServiceDto>> GetUpcomingServicesAsync(DateTime date)
    {
        var endDate = date.AddDays(14);
        var sql = @"
            SELECT
                c.Id AS CarId,
                c.Type,
                c.Make,
                c.Model,
                c.Year,
                c.Location,
                s.Id AS ServiceId,
                s.Date
            FROM
                Services s
            JOIN
                Cars c ON s.CarId = c.Id
            WHERE
                s.Date >= @startDate AND s.Date <= @endDate
            ORDER BY
                s.Date ASC;";

        using var connection = _connectionFactory.CreateConnection();
        var dynamicResults = await connection.QueryAsync(sql, new { startDate = date, endDate = endDate });

        var cars = new List<CarWithServiceDto>();
        foreach (dynamic item in dynamicResults)
        {
            if (item.ServiceId is not Guid serviceGuid)
            {
                if (!Guid.TryParse(item.ServiceId?.ToString().Trim(), out serviceGuid))
                {
                    throw new Exception("ServiceId is not a Guid");
                }
            }

            var service = new ServiceDto(
                ServiceId: serviceGuid,
                Date: DateTime.Parse(item.Date)
            );

            if (item.CarId is not Guid carGuid)
            {
                if (!Guid.TryParse(item.CarId?.ToString().Trim(), out carGuid))
                {
                    throw new Exception("CarId is not a Guid");
                }
            }

            cars.Add(new CarWithServiceDto(
                CarId: carGuid,
                Type: (string)item.Type,
                Make: (string)item.Make,
                Model: (string)item.Model,
                Year: (int)item.Year,
                Location: (string)item.Location,
                Service: service
            ));
        }

        return cars;
    }

    public async Task<IEnumerable<TopRentedCarTypeDto>> GetTopRentedCarTypesAsync(DateTime startDate, DateTime endDate)
    {
        const string sql = @"
            WITH RentalCounts AS (
                SELECT
                    ca.Type,
                    COUNT(r.Id) AS RentalCount
                FROM
                    Rentals r
                JOIN
                    Cars ca ON r.CarId = ca.Id
                WHERE
                    r.StartDate >= @startDate AND r.StartDate <= @endDate
                GROUP BY
                    ca.Type
            ),
            TotalRentals AS (
                SELECT
                    SUM(RentalCount) AS Total
                FROM
                    RentalCounts
            )
            SELECT
                rc.Type AS CarType,
                CAST(rc.RentalCount AS REAL) / tr.Total * 100 AS UtilizationPercentage
            FROM
                RentalCounts rc, TotalRentals tr
            ORDER BY
                rc.RentalCount DESC
            LIMIT 3;";

        using var connection = _connectionFactory.CreateConnection();
        var dynamicResults = await connection.QueryAsync<dynamic>(sql, new { startDate, endDate });

        var results = new List<TopRentedCarTypeDto>();
        foreach (var item in dynamicResults)
        {
            results.Add(new TopRentedCarTypeDto(
                CarType: (string)item.CarType,
                UtilizationPercentage: (decimal)item.UtilizationPercentage
            ));
        }

        return results;
    }

    public async Task<PaginatedResult<TopRentedCarDto>> GetTopRentedCarsAsync(string? make, string? model, string? type, string? location, int limit, int offset)
    {
        var parameters = new DynamicParameters();
        var whereClauses = new List<string>();

        if (!string.IsNullOrWhiteSpace(make))
        {
            whereClauses.Add("ca.Make = @make");
            parameters.Add("make", make);
        }
        if (!string.IsNullOrWhiteSpace(model))
        {
            whereClauses.Add("ca.Model = @model");
            parameters.Add("model", model);
        }
        if (!string.IsNullOrWhiteSpace(type))
        {
            whereClauses.Add("ca.Type = @type");
            parameters.Add("type", type);
        }
        if (!string.IsNullOrWhiteSpace(location))
        {
            whereClauses.Add("ca.Location = @location");
            parameters.Add("location", location);
        }

        var whereClause = whereClauses.Any() ? "WHERE " + string.Join(" AND ", whereClauses) : string.Empty;

        var sql = $@"
            SELECT
                ca.Id AS CarId,
                ca.Type,
                ca.Make,
                ca.Model,
                ca.Year,
                COUNT(r.Id) AS RentalCount
            FROM
                Cars ca
            LEFT JOIN
                Rentals r ON ca.Id = r.CarId
            {whereClause}
            GROUP BY
                ca.Id, ca.Type, ca.Make, ca.Model, ca.Year
            ORDER BY
                RentalCount DESC
            LIMIT @limit OFFSET @offset;
            
            SELECT
                COUNT(DISTINCT ca.Id)
            FROM
                Cars ca
            LEFT JOIN
                Rentals r ON ca.Id = r.CarId
            {whereClause};";

        parameters.Add("limit", limit);
        parameters.Add("offset", offset);

        using var connection = _connectionFactory.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(sql, parameters);

        var dynamicResults = await multi.ReadAsync<dynamic>();
        var totalCount = await multi.ReadSingleAsync<int>();

        var parsedResults = new List<TopRentedCarDto>();
        foreach (var item in dynamicResults)
        {
            var car = new CarDto(
                CarId: new Guid((string)item.CarId),
                Type: (string)item.Type,
                Make: (string)item.Make,
                Model: (string)item.Model,
                Year: (int)item.Year
            );

            parsedResults.Add(new TopRentedCarDto(
                Car: car,
                RentalCount: (int)item.RentalCount
            ));
        }

        return new PaginatedResult<TopRentedCarDto>(parsedResults, limit, offset, totalCount);
    }
}