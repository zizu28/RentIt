using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Verification.Application.Commands;

namespace RentIt.Modules.Verification.Api.Controllers;

[ApiController]
[Route("api/verification")]
public class VerificationController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitVerification([FromBody] SubmitVerificationCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { VerificationId = result.Value });
        }
        
        return BadRequest(result.Error);
    }

    [HttpPost("{verificationId}/approve")]
    public async Task<IActionResult> ApproveVerification(Guid verificationId, [FromBody] ApproveVerificationDto dto)
    {
        var command = new ApproveVerificationCommand(verificationId, dto.Comments);
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok();
        }
        
        return BadRequest(result.Error);
    }

    [HttpPost("{verificationId}/reject")]
    public async Task<IActionResult> RejectVerification(Guid verificationId, [FromBody] RejectVerificationDto dto)
    {
        var command = new RejectVerificationCommand(verificationId, dto.Comments);
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok();
        }
        
        return BadRequest(result.Error);
    }
}

public class ApproveVerificationDto
{
    public string? Comments { get; set; }
}

public class RejectVerificationDto
{
    public string Comments { get; set; } = string.Empty;
}
