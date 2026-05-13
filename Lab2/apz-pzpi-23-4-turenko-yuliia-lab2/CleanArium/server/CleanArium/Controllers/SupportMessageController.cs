using Application.Abstractions;
using Application.SupportMessages.Commands.Create;
using Application.SupportMessages.Commands.Delete;
using Application.SupportMessages.Commands.Reply;
using Application.SupportMessages.Commands.Update;
using Application.SupportMessages.Queries.GetAllByUserId;
using Application.SupportMessages.Queries.GetAllForAdmin;
using Application.SupportMessages.Queries.GetHistory;
using CleanArium.Contracts.SupportMessages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArium.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupportMessageController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;

    public SupportMessageController(IMediator mediator, IUserService userService)
    {
        _mediator = mediator;
        _userService = userService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateMessageRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new CreateMessageCommand(
            UserId: userId,
            Subject: request.Subject,
            Message: request.Message
        );

        var result = await _mediator.Send(command, ct);

        return Ok(result);
    }

    [HttpPost("reply")]
    public async Task<IActionResult> Reply([FromBody] ReplyMessageRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new ReplyToMessageCommand(
            UserId: userId,
            MessageId: request.MessageId,
            Subject: request.Subject,
            Message: request.Message
        );

        var result = await _mediator.Send(command, ct);

        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateMessageRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new UpdateMessageCommand(
            UserId: userId,
            MessageId: request.Id,
            Subject: request.Subject,
            Message: request.Message
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{firstMessageId:long}")]
    public async Task<IActionResult> Delete(long firstMessageId, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new DeleteMessagesCommand(
            UserId: userId,
            FirstMessageId: firstMessageId
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-all-for-admin")]
    public async Task<IActionResult> GetAllForAdmin(CancellationToken ct)
    {
        var query = new GetAllMessagesForAdminQuery();
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    [HttpGet("get-all-by-user")]
    public async Task<IActionResult> GetAllByUserId(CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var query = new GetAllMessagesByUserIdQuery(userId);
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    [HttpGet("get-history/{firstMessageId:long}")]
    public async Task<IActionResult> GetHistoryByFirstMessageId(long firstMessageId, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var query = new GetHistoryOfMessagesQuery(userId, firstMessageId);
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }
}
