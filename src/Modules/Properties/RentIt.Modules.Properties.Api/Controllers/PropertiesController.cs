using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Properties.Application.Commands;
using RentIt.Modules.Properties.Application.Queries;
using System.Security.Claims;

namespace RentIt.Modules.Properties.Api.Controllers;

[ApiController]
[Route("api/properties")]
[Authorize]
public class PropertiesController(IMediator mediator, Serilog.ILogger logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly Serilog.ILogger _logger = logger;

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
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyCommand command)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var hostId))
        {
            _logger.Warning("Failed to parse user ID from token in CreateProperty");
            return Unauthorized();
        }

        // Ensure the host ID in the command matches the authenticated user
        if (command.HostId != hostId)
        {
            command = command with { HostId = hostId };
        }

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetHostProperties), new { id = result.Value }, result.Value);
        }

        return BadRequest(result.Error);
    }
}
