using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Devices.Commands.Update;

public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceCommand>
{
    private readonly IDeviceRepository _repo;
    private readonly ILogger<UpdateDeviceCommandHandler> _logger;

    public UpdateDeviceCommandHandler(IDeviceRepository repo, ILogger<UpdateDeviceCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(UpdateDeviceCommand request, CancellationToken ct)
    {
        var device = await _repo.GetByIdAsync(request.DeviceId);

        device.DeviceType = request.DeviceType;
        device.DeviceStatus = request.DeviceStatus;

        await _repo.UpdateAsync(device);

        _logger.LogInformation("USER_ACTION Device {Id} updated", request.DeviceId);
    }
}
