using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Application.Queries;
using RentIt.Shared.DTOs.Bookings;
using System.Security.Claims;

namespace RentIt.Modules.Bookings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        var guestIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(guestIdString) || !Guid.TryParse(guestIdString, out var guestId))
        {
            return Unauthorized("User is not authenticated properly.");
        }

        var command = new CreateBookingCommand(request.PropertyId, guestId, request.StartDate, request.EndDate);
        
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // For now just return BadRequest with message, ideally handled by GlobalExceptionHandler
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings()
    {
        var guestIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(guestIdString) || !Guid.TryParse(guestIdString, out var guestId))
        {
            return Unauthorized("User is not authenticated properly.");
        }

        var query = new GetMyBookingsQuery(guestId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("properties/{propertyId}/booked-periods")]
    public async Task<IActionResult> GetPropertyBookedPeriods(Guid propertyId)
    {
        var query = new GetPropertyBookedPeriodsQuery(propertyId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

public record CreateBookingRequest(Guid PropertyId, DateOnly StartDate, DateOnly EndDate);
