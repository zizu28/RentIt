using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Identity.Application.Queries;

using RentIt.Modules.Identity.Application.Commands;
using RentIt.Shared.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;

namespace RentIt.Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/identity/users")]
[Authorize]
public sealed class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
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

        var query = new GetUserQuery(Id);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error.Message });
    }

    [HttpPut("me/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
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

        var command = new UpdateUserProfileCommand(Id, request.FirstName, request.LastName, request.Address);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { error = result.Error.Message });
    }
}
