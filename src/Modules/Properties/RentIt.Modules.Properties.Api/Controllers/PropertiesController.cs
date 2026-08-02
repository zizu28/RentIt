using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Properties.Application.Commands;
using RentIt.Modules.Properties.Application.Queries;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RentIt.Shared.DTOs.Properties;

namespace RentIt.Modules.Properties.Api.Controllers;

[ApiController]
[Route("api/properties")]
[Authorize]
public class PropertiesController(
    IMediator mediator,
    Serilog.ILogger logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly Serilog.ILogger _logger = logger;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProperties()
    {
        var query = new GetAllPropertiesQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPropertyById(Guid id)
    {
        var query = new GetPropertyByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.Error == "Property not found")
        {
            return NotFound(result.Error);
        }

        return BadRequest(result.Error);
    }

    [HttpGet("host")]
    public async Task<IActionResult> GetHostProperties()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var hostId))
        {
            _logger.Warning("Failed to parse user ID from token in GetHostProperties");
            return Unauthorized();
        }

        var query = new GetPropertiesByHostIdQuery(hostId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProperty([FromForm] CreatePropertyApiRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var hostId))
        {
            _logger.Warning("Failed to parse user ID from token in CreateProperty");
            return Unauthorized();
        }

        var fileRecords = new List<CreatePropertyCommand.FileRecord>();
        if (request.Images != null)
        {
            foreach (var file in request.Images)
            {
                if (file.Length > 0)
                {
                    fileRecords.Add(new CreatePropertyCommand.FileRecord(file.OpenReadStream(), file.FileName));
                }
            }
        }

        var command = new CreatePropertyCommand(hostId, request.Name, request.Description,
            request.Street, request.City, request.Region, request.Country, request.PostalCode,
            request.Type, request.RentalPeriod, request.PricePerPeriod, request.SecurityDeposit, request.Bedrooms,
            request.Bathrooms, request.Amenities ?? [], fileRecords, request.Status == 0 ? 1 : request.Status);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetHostProperties), new { id = result.Value }, result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var hostId))
        {
            _logger.Warning("Failed to parse user ID from token in UpdateProperty");
            return Unauthorized();
        }

        var command = new UpdatePropertyCommand(
            id,
            hostId,
            request.Name,
            request.Description,
            request.Street,
            request.City,
            request.Region,
            request.Country,
            request.PostalCode,
            request.Type,
            request.RentalPeriod,
            request.PricePerPeriod,
            request.SecurityDeposit,
            request.Bedrooms,
            request.Bathrooms,
            request.Amenities ?? Array.Empty<string>(),
            request.Status
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        if (result.Error == "Property not found." || result.Error == "You do not have permission to edit this property.")
        {
            return NotFound(result.Error); // Could also be Forbidden for permission
        }

        return BadRequest(result.Error);
    }
}

public class CreatePropertyApiRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public int Type { get; set; }
    public int Status { get; set; }
    public int RentalPeriod { get; set; }
    public decimal PricePerPeriod { get; set; }
    public decimal SecurityDeposit { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public IEnumerable<string>? Amenities { get; set; }
    public IFormFileCollection? Images { get; set; }
}
