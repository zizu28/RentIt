using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using RentIt.Modules.Identity.Application.Queries;

using RentIt.Modules.Identity.Application.Commands;
using RentIt.Shared.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using RentIt.Shared.Abstractions.Storage;
using Microsoft.AspNetCore.Http;

namespace RentIt.Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/identity/users")]
[Authorize]
[EnableRateLimiting("api")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("me")]
    [OutputCache(PolicyName = "short")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return Unauthorized("Token does not contain a User ID claim.");
        }

        if (!Guid.TryParse(userIdClaim.Value, out Guid Id))
        {
            return Unauthorized($"Token User ID {Id} is not a valid GUID.");
        }

        var command = new UpdateUserProfileCommand(Id, request.FirstName, request.LastName, request.Address, request.Phone);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { error = result.Error.Message });
    }

    [HttpPost("me/profile-image")]
    public async Task<IActionResult> UploadProfileImage(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return Unauthorized("Token does not contain a User ID claim.");
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "No file uploaded." });
        }

        // Limit to 5MB
        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest(new { error = "File size exceeds the 5MB limit." });
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var command = new UpdateProfileImageCommand(userIdClaim.Value, stream, file.FileName);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(new { error = result.Error.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while uploading the image.", details = ex.Message });
        }
    }
}
