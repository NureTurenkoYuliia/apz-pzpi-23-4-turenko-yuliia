using Application.Users.Commands.Block;
using Application.Users.Commands.Delete;
using Application.Users.Commands.Unblock;
using Application.Users.Queries.GetAll;
using Application.Users.Queries.GetById;
using Application.Users.Queries.GetInactive;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost("block/{id}")]
    public async Task<IActionResult> BlockUser(int id, CancellationToken ct)
    {
        var command = new BlockUserCommand(id);

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost("ublock/{id}")]
    public async Task<IActionResult> UnblockUser(int id, CancellationToken ct)
    {
        var command = new UnblockUserCommand(id);

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
    {
        var command = new DeleteUserCommand(id);

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllUsersQuery());

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet("inactive-users/{inactiveDays:int}")]
    public async Task<IActionResult> GetInactiveUsers(CancellationToken ct, [FromRoute] int inactiveDays = 60)
    {
        var result = await _mediator.Send(new GetInactiveUsersQuery(inactiveDays));

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet("{id}/user")]
    public async Task<IActionResult> GetUserById(CancellationToken ct, [FromRoute] long id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));

        return Ok(result);
    }
}
