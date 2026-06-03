using Application.Abstractions;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Devices.Commands.Create;

public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, long>
{
    private readonly IDeviceRepository _repo;
    private readonly ILogger<CreateDeviceCommandHandler> _logger;

    public CreateDeviceCommandHandler(IDeviceRepository repo, ILogger<CreateDeviceCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<long> Handle(CreateDeviceCommand request, CancellationToken ct)
    {
        var device = new Device
        {
            AquariumId = request.AquariumId,
            DeviceType = request.DeviceType,
            DeviceStatus = request.DeviceStatus
        };

        await _repo.AddAsync(device);

        _logger.LogInformation("USER_ACTION Device {Id} created in Aquarium {Aquarium}", device.Id, request.AquariumId);

        return device.Id;
    }
}
