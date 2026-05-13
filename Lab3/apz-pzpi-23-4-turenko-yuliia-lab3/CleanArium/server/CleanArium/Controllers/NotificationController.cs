using Application.Abstractions;
using Application.Notifications.Commands.MarkAsRead;
using Application.Notifications.Queries.GetAllByUserId;
using Application.Notifications.Queries.GetUnreadCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArium.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;

    public NotificationController(IMediator mediator, IUserService userService)
    {
        _mediator = mediator;
        _userService = userService;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetUserNotifications(CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;
        var query = new GetNotificationsQuery ( UserId: userId );
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    [HttpPost("read/{id:long}")]
    public async Task<IActionResult> MarkAsRead(long id, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var cmd = new MarkAsReadCommand (UserId: userId, NotificationId: id);

        await _mediator.Send(cmd, ct);
        return Ok();
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var result = await _mediator.Send(
            new GetUnreadNotificationsCountQuery(userId),ct);

        return Ok(result);
    }
}
