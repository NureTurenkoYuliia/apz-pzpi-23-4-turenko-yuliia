using Application.Abstractions;
using Application.AlarmRules.Commands.Create;
using Application.AlarmRules.Commands.Deactivate;
using Application.AlarmRules.Commands.Delete;
using Application.AlarmRules.Commands.Import;
using Application.AlarmRules.Commands.Update;
using Application.AlarmRules.Queries.GetAllByDeviceId;
using Application.AlarmRules.Queries.GetById;
using CleanArium.Contracts.AlarmRules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlarmRuleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;
    private readonly IAlarmRuleAnalyticsService _analiticsService;
    private readonly IAlarmRuleRepository _repo;

    public AlarmRuleController(
        IMediator mediator, 
        IUserService userService, 
        IAlarmRuleAnalyticsService analiticsService,
        IAlarmRuleRepository repo)
    {
        _mediator = mediator;
        _userService = userService;
        _analiticsService = analiticsService;
        _repo = repo;
    }

    [Authorize(Roles = "User")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateAlarmRuleRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new CreateAlarmRuleCommand(
            UserId: userId,
            DeviceId: request.DeviceId,
            Condition: request.Condition,
            Threshold: request.Threshold,
            Unit: request.Unit
        );

        var result = await _mediator.Send(command, ct);

        return Ok(result);
    }

    [Authorize(Roles = "User")]
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateAlarmRuleRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new UpdateAlarmRuleCommand(
            UserId: userId,
            RuleId: request.RuleId,
            Condition: request.Condition,
            Threshold: request.Threshold,
            Unit: request.Unit,
            IsActive: request.IsActive
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "User")]
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] long ruleId, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new DeleteAlarmRuleCommand(
            UserId: userId,
            RuleId: ruleId
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "User,Admin,Moderator")]
    [HttpGet("get-all-by-device/{deviceId:long}")]
    public async Task<IActionResult> GetAllByDevice(long deviceId, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var query = new GetAlarmRulesByDeviceIdQuery(userId, deviceId);
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    [Authorize(Roles = "User,Admin,Moderator")]
    [HttpGet("get-by-id/{ruleId:long}")]
    public async Task<IActionResult> GetById(long ruleId, CancellationToken ct)
    {
        var query = new GetAlarmRuleByIdQuery(ruleId);
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    [Authorize(Roles = "User")]
    [HttpPost("{deviceId}/import")]
    public async Task<IActionResult> ImportRules(long deviceId, IFormFile file)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var result = await _mediator.Send(new ImportAlarmRulesCommand(userId, deviceId, file));

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateAlarmRule(long id)
    {
        await _mediator.Send(new DeactivateAlarmRuleCommand(id));
        return Ok("AlarmRule deactivated");
    }

    [Authorize(Roles = "User,Admin,Moderator")]
    [HttpGet("{id}/analysis")]
    public async Task<IActionResult> AnalyzeAlarmRule(long id, DateTime from, DateTime to, CancellationToken ct)
    {
        var rule = await _repo.GetByIdAsync(id, ct);
        if (rule == null)
            return NotFound();

        var result = await _analiticsService.AnalyzeAsync(rule, from, to);
        return Ok(result);
    }
}
