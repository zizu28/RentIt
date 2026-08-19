using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Analytics.Application.Queries;

namespace RentIt.Modules.Analytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("properties/{propertyId:guid}")]
    public async Task<IActionResult> GetPropertyStats(Guid propertyId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPropertyStatsQuery(propertyId), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }
}
