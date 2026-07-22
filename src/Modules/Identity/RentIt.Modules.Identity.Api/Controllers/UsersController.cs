using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Identity.Application.Queries;

namespace RentIt.Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/identity/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var identifier = JwtRegisteredClaimNames.Sub ?? ClaimTypes.NameIdentifier;
        var userIdClaim = User.FindFirst(identifier);
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return Unauthorized("Token does not contain a User ID claim.");
        }

        if (!Guid.TryParse(userIdClaim.Value, out Guid Id))
        {
            return Unauthorized($"Token User ID {Id} is not a valid GUID.");
        }

        var query = new GetUserQuery(userId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error.Message });
    }
}
