using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Application.Queries;

namespace RentIt.Modules.Bookings.Api.Controllers;

[ApiController]
[Route("api/bookings/properties")]
[Authorize]
public class BookablePropertiesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBookablePropertyById(Guid id)
    {
        var query = new GetBookablePropertyByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error == "Bookable property not found.")
        {
            return NotFound(result.Error);
        }

        return BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBookableProperty([FromBody] CreateBookablePropertyRequest request)
    {
        var command = new CreateBookablePropertyCommand(
            request.PropertyId,
            request.Title,
            request.ImageUrl,
            request.PricePerNight,
            request.Currency,
            request.RentalPeriod,
            request.HostId
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetBookablePropertyById), new { id = result.Value }, result.Value);
        }

        return BadRequest(result.Error);
    }
}

public class CreateBookablePropertyRequest
{
    public Guid PropertyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string Currency { get; set; } = "GHS";
    public int RentalPeriod { get; set; }
    public Guid HostId { get; set; }
}
