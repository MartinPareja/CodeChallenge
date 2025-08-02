using CarRental.Application.Commands.Rentals.CancelRental;
using CarRental.Application.Commands.Rentals.ModifyReservation;
using CarRental.Application.Commands.Rentals.RegisterRental;
using CarRental.Application.Queries.Cars.CheckAvailability;
using CarRental.Application.Queries.Rentals.GetRentalMetrics;
using CarRental.Application.Queries.Rentals.GetRentalsForUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CarRental.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly ISender _sender;

    public RentalsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckAvailability(
        DateTime startDate, 
        DateTime endDate, 
        int year, 
        string make = "", 
        string model = "", 
        string location = "", 
        string type = "",
        int limit = 10,
        int offset = 0)
    {
        var query = new CheckAvailabilityQuery(startDate, endDate, make, model, year, location, type, limit, offset);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterRental([FromBody] RegisterRentalCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(RegisterRental), new { rentalId = result.RentalId }, result);
    }

    [HttpPut("{rentalId}/modify")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ModifyReservation(Guid rentalId, [FromBody] ModifyReservationCommand command)
    {
        if (rentalId != command.RentalId)
        {
            return BadRequest("Rental ID in route does not match command ID.");
        }
        await _sender.Send(command);
        return NoContent();
    }

    [HttpPost("{rentalId}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelRental(Guid rentalId)
    {
        var command = new CancelRentalCommand(rentalId);
        await _sender.Send(command);
        return NoContent();
    }

    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRentalsForUser([FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var requestingUserId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var query = new GetRentalsForUserQuery(requestingUserId, limit, offset);
        var rentals = await _sender.Send(query);

        return Ok(rentals);
    }

    [HttpGet("metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRentalMetrics([FromQuery] DateTime? date)
    {
        var query = new GetRentalMetricsQuery(date ?? DateTime.Now);
        var metrics = await _sender.Send(query);

        return Ok(metrics);
    }
}