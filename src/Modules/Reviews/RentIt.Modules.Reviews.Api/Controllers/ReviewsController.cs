using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Reviews.Application.Commands;
using RentIt.Modules.Reviews.Application.Queries;

namespace RentIt.Modules.Reviews.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] AddReviewCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { ReviewId = result.Value });
        }
        
        return BadRequest(result.Error);
    }

    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetPropertyReviews(Guid propertyId)
    {
        var query = new GetPropertyReviewsQuery(propertyId);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return BadRequest(result.Error);
    }
}
