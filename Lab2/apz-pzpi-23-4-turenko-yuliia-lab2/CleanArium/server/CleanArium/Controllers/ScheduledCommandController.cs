using Application.Abstractions;
using Application.ScheduledCommands.Commands.Create;
using Application.ScheduledCommands.Commands.Deactivate;
using Application.ScheduledCommands.Commands.Delete;
using Application.ScheduledCommands.Commands.Import;
using Application.ScheduledCommands.Commands.Update;
using Application.ScheduledCommands.Queries.GetAllByDeviceId;
using Application.ScheduledCommands.Queries.GetById;
using CleanArium.Contracts.ScheduledCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduledCommandController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;

    public ScheduledCommandController(IMediator mediator, IUserService userService)
    {
        _mediator = mediator;
        _userService = userService;
    }

    [Authorize(Roles = "User")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateScheduledCommandRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new CreateScheduledCommand(
            UserId: userId,
            DeviceId: request.DeviceId,
            CommandType: request.CommandType,
            StartTime: request.StartTime,
            RepeatMode: request.RepeatMode,
            IntervalMinutes: request.IntervalMinutes,
            IsActive: request.IsActive
        );

        var result = await _mediator.Send(command, ct);

        return Ok(result);
    }

    [Authorize(Roles = "User")]
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateScheduledCommandRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new UpdateScheduledCommand(
            UserId: userId,
            CommandId: request.Id,
            CommandType: request.CommandType,
            StartTime: request.StartTime,
            RepeatMode: request.RepeatMode,
            IntervalMinutes: request.IntervalMinutes,
            IsActive: request.IsActive
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "User")]
    [HttpDelete("delete/{scheduledCommandId:long}")]
    public async Task<IActionResult> Delete(long scheduledCommandId, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new DeleteScheduledCommand(
            UserId: userId,
            CommandId: scheduledCommandId
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "User,Admin,Moderator")]
    [HttpGet("get-all-by-device/{deviceId:long}")]
    public async Task<IActionResult> GetAllByDevice(long deviceId, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var query = new GetScheduledCommandsByDeviceIdQuery(userId, deviceId);
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    [Authorize(Roles = "User,Admin,Moderator")]
    [HttpGet("get-by-id/{scheduledCommandId:long}")]
    public async Task<IActionResult> GetById(long scheduledCommandId, CancellationToken ct)
    {
        var query = new GetScheduledCommandByIdQuery(scheduledCommandId);
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    [Authorize(Roles = "User")]
    [HttpPost("{deviceId}/import")]
    public async Task<IActionResult> ImportScheduled(long deviceId, IFormFile file)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var result = await _mediator.Send(new ImportScheduledCommandsCommand(userId, deviceId, file));

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateScheduled(long id)
    {
        await _mediator.Send(new DeactivateScheduledCommand(id));

        return Ok();
    }
}
