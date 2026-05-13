using Application.Abstractions;
using Application.Devices.Commands.Create;
using Application.Devices.Commands.Delete;
using Application.Devices.Commands.Update;
using Application.Devices.Queries.GetAllByAquariumId;
using Application.ExecutedCommands.Commands.Create;
using Application.ExecutedCommands.Queries.GetAllByDeviceId;
using Application.SensorData.Commands.Create;
using Application.SensorData.Queries.GetLatestByDeviceId;
using CleanArium.Contracts.Devices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;

    public DeviceController(IMediator mediator, IUserService userService)
    {
        _mediator = mediator;
        _userService = userService;
    }

    [Authorize(Roles = "User")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDeviceRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new CreateDeviceCommand(
            UserId: userId,
            AquariumId: request.AquariumId,
            DeviceType: request.DeviceType,
            DeviceStatus: request.DeviceStatus
        );

        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [Authorize(Roles = "User")]
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateDeviceRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new UpdateDeviceCommand(
            UserId: userId,
            DeviceId: request.DeviceId,
            DeviceType: request.DeviceType,
            DeviceStatus: request.DeviceStatus
        );

        await _mediator.Send(command, ct);
        return Ok();
    }

    [Authorize(Roles = "User,Admin,Moderator")]
    [HttpDelete("delete/{deviceId:long}")]
    public async Task<IActionResult> Delete([FromRoute] long deviceId, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new DeleteDeviceCommand(
            UserId: userId,
            DeviceId: deviceId
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    [Authorize(Roles = "User")]
    [HttpGet("get-devices-by-aquarium/{aquariumId:long}")]
    public async Task<IActionResult> GetAllByAquarium(long aquariumId, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var query = new GetDevicesByAquariumIdQuery(
            UserId: userId,
            AquariumId: aquariumId
        );

        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [Authorize(Roles = "User")]
    [HttpGet("get-sensor-data/{deviceId:long}")]
    public async Task<IActionResult> GetLatestSensorData(long deviceId, CancellationToken ct)
    {
        var query = new GetLatestSensorDataByDeviceIdQuery(deviceId);
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    [HttpPost("{deviceId:long}/sensor-data")]
    public async Task<IActionResult> AddSensorData([FromRoute] long deviceId, CreateSensorDataRequest request, CancellationToken ct)
    {
        var command = new CreateSensorDataCommand(
            DeviceId: deviceId,
            Value: request.Value,
            Unit: request.Unit
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    [HttpPost("{deviceId:long}/executed-command")]
    public async Task<IActionResult> AddExecutedCommand([FromRoute] long deviceId, CreateExecutedCommandRequest request, CancellationToken ct)
    {
        var command = new CreateExecutedCommand(
            DeviceId: deviceId,
            CommandType: request.CommandType,
            CommandStatus: request.CommandStatus
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    [HttpGet("executed-commands-by-device/{deviceId:long}")]
    public async Task<IActionResult> GetAllExecutedCommandsByDevice(long deviceId, CancellationToken ct)
    {
        var query = new GetExecutedCommandsByDeviceIdQuery(deviceId);
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }
}
