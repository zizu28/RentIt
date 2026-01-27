using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Identity.Application.Queries;

namespace RentIt.Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/identity/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(userId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error.Message });
    }
}
