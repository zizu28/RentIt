using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Payments.Application.Commands;

namespace RentIt.Modules.Payments.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("initialize")]
    [Authorize]
    public async Task<IActionResult> InitializePayment([FromBody] InitializePaymentRequest request, CancellationToken cancellationToken)
    {
        var command = new InitializePaymentCommand(
            request.BookingId,
            request.Amount,
            request.Currency,
            request.Email,
            request.CallbackUrl
        );

        var authorizationUrl = await _mediator.Send(command, cancellationToken);
        return Ok(new { AuthorizationUrl = authorizationUrl });
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
}

public record InitializePaymentRequest(Guid BookingId, decimal Amount, string Currency, string Email, string CallbackUrl);
