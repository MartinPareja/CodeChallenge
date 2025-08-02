namespace CarRental.Api.Controllers;

using CarRental.Application.Common;
using CarRental.Application.Queries.Cars.GetTopRentedCars;
using CarRental.Application.Queries.Cars.GetTopRentedCarTypes;
using CarRental.Application.Queries.Cars.GetUpcomingServices;
using CsvHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ISender _sender;

    public CarsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("upcoming-services")]
    [Produces("application/json", "text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUpcomingCarServices([FromQuery] DateTime? date = null, [FromHeader(Name = "Accept")] string acceptHeader = "application/json")
    {
        var query = new GetUpcomingCarServicesQuery(date ?? DateTime.UtcNow.Date);
        var cars = await _sender.Send(query);

        if (acceptHeader.Contains("text/csv"))
        {
            var csv = GenerateCsv(cars);
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", "upcoming-services.csv");
        }

        return Ok(cars);
    }

    private string GenerateCsv(IEnumerable<CarWithServiceDto> cars)
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(cars);
        return writer.ToString();
    }

    [HttpGet("top-types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTopRentedCarTypes(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        if (startDate == null || endDate == null || startDate > endDate)
        {
            return BadRequest("Invalid date range.");
        }

        var query = new GetTopRentedCarTypesQuery(startDate.Value, endDate.Value);
        var result = await _sender.Send(query);

        return Ok(result);
    }

    [HttpGet("top")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopRentedCars(
        [FromQuery] string? make,
        [FromQuery] string? model,
        [FromQuery] string? type,
        [FromQuery] string? location,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0)
    {
        var query = new GetTopRentedCarsQuery(make, model, type, location, limit, offset);
        var result = await _sender.Send(query);

        return Ok(result);
    }
}