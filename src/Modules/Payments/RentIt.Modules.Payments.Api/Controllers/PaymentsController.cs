using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Payments.Application.Commands;
using System.Security.Claims;

namespace RentIt.Modules.Payments.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("initialize")]
    [Authorize(Policy = "RequireGuest")]
    public async Task<IActionResult> InitializePayment([FromBody] InitializePaymentRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        if (userId == Guid.Empty) return Unauthorized();

        var command = new InitializePaymentCommand(
            userId,
            request.BookingId,
            request.Amount,
            request.Currency,
            request.Email,
            request.CallbackUrl
        );

        var authorizationUrl = await _mediator.Send(command, cancellationToken);
        return Ok(new { AuthorizationUrl = authorizationUrl, Status = true });
    }

    [HttpPost("webhook")]
    [AllowAnonymous] // Webhooks come from outside
    public async Task<IActionResult> HandleWebhook([FromBody] PaystackWebhookPayload payload, CancellationToken cancellationToken)
    {
        // In a real app, you MUST verify the Paystack signature here using the Secret Key 
        // Request.Headers["x-paystack-signature"]

        var command = new ProcessPaymentWebhookCommand(payload);
        await _mediator.Send(command, cancellationToken);

        return Ok(); // Paystack expects a 200 OK
    }

    [HttpPost("verify/{reference}")]
    [Authorize(Policy = "RequireGuest")]
    public async Task<IActionResult> VerifyPayment(string reference, CancellationToken cancellationToken)
    {
        var command = new VerifyPaymentCommand(reference);
        var result = await _mediator.Send(command, cancellationToken);
        
        return Ok(new { Success = result });
    }

    [HttpGet("booking/{bookingId}")]
    [Authorize]
    public async Task<IActionResult> GetPaymentByBookingId(Guid bookingId, CancellationToken cancellationToken)
    {
        var query = new RentIt.Modules.Payments.Application.Queries.GetPaymentByBookingIdQuery(bookingId);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
        {
            return NotFound(new { Message = "Payment not found for this booking." });
        }

        return Ok(result);
    }

    [HttpGet("methods")]
    [Authorize(Policy = "RequireGuest")]
    public async Task<IActionResult> GetPaymentMethods(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        if (userId == Guid.Empty) return Unauthorized();

        var query = new RentIt.Modules.Payments.Application.Queries.GetPaymentMethodsQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("methods/setup")]
    [Authorize(Policy = "RequireGuest")]
    public async Task<IActionResult> SetupPaymentMethod([FromBody] SetupPaymentMethodRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        if (userId == Guid.Empty) return Unauthorized();

        var command = new SetupPaymentMethodCommand(userId, request.Currency);
        var authorizationUrl = await _mediator.Send(command, cancellationToken);
        
        return Ok(new { AuthorizationUrl = authorizationUrl });
    }
}

public record SetupPaymentMethodRequest(string Currency);

public record InitializePaymentRequest(Guid BookingId, decimal Amount, string Currency, string Email, string CallbackUrl);
