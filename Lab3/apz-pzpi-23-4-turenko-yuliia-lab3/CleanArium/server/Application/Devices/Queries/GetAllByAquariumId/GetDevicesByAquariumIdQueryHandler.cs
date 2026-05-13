using Application.Abstractions;
using Application.DTOs.Devices;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Devices.Queries.GetAllByAquariumId;

public class GetDevicesByAquariumIdQueryHandler : IRequestHandler<GetDevicesByAquariumIdQuery, List<DeviceDto>>
{
    private readonly IDeviceRepository _repo;
    private readonly ILogger<GetDevicesByAquariumIdQueryHandler> _logger;

    public GetDevicesByAquariumIdQueryHandler(IDeviceRepository repo, ILogger<GetDevicesByAquariumIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<DeviceDto>> Handle(GetDevicesByAquariumIdQuery request, CancellationToken ct)
    {
        var devices = await _repo.GetByAquariumIdAsync(request.AquariumId);

        List<DeviceDto> list = devices.Select(a => new DeviceDto
        {
            Id = a.Id,
            AquariumId = a.AquariumId,
            DeviceType = a.DeviceType,
            DeviceStatus = a.DeviceStatus
        })
        .ToList();

        _logger.LogInformation("USER_ACTION Successfully retrieved devices for aquarium: {Id} ", request.AquariumId);

        return list;
    }
}
