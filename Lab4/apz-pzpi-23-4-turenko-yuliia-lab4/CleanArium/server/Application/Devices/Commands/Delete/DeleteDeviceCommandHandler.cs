using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Devices.Commands.Delete;

public class DeleteDeviceCommandHandler : IRequestHandler<DeleteDeviceCommand>
{
    private readonly IDeviceRepository _repo;
    private readonly ILogger<DeleteDeviceCommandHandler> _logger;

    public DeleteDeviceCommandHandler(IDeviceRepository repo, ILogger<DeleteDeviceCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(DeleteDeviceCommand request, CancellationToken ct)
    {
        var device = await _repo.GetByIdAsync(request.DeviceId);

        await _repo.DeleteAsync(device);

        _logger.LogWarning("USER_ACTION Device {Id} deleted", request.DeviceId);
    }
}
