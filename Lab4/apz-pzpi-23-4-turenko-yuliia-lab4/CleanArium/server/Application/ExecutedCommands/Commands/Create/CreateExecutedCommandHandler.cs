using Application.Abstractions;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.ExecutedCommands.Commands.Create;

public class CreateExecutedCommandHandler : IRequestHandler<CreateExecutedCommand>
{
    private readonly IExecutedCommandRepository _repo;
    private readonly IDeviceRepository _deviceRepo;
    private readonly ILogger<CreateExecutedCommandHandler> _logger;

    public CreateExecutedCommandHandler(IExecutedCommandRepository repo,
        IDeviceRepository deviceRepo, ILogger<CreateExecutedCommandHandler> logger)
    {
        _repo = repo;
        _deviceRepo = deviceRepo;
        _logger = logger;
    }

    public async Task Handle(CreateExecutedCommand command, CancellationToken ct)
    {
        var entity = new ExecutedCommand
        {
            DeviceId = command.DeviceId,
            CommandType = command.CommandType,
            CommandStatus = command.CommandStatus,
            IssuedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);

        var device = await _deviceRepo.GetByIdAsync(command.DeviceId);

        if (command.CommandType == CommandType.TurnOn)
        {
            device.DeviceStatus = DeviceStatus.On;
        }
        else if (command.CommandType == CommandType.TurnOff)
        {
            device.DeviceStatus = DeviceStatus.Off;
        }

        await _deviceRepo.UpdateAsync(device);

        _logger.LogInformation("USER_ACTION Execute command {CommandType} for device: {DeviceId}", command.CommandType, command.DeviceId);
    }
}
