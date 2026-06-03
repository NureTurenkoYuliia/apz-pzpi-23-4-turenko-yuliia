using Application.Abstractions;
using Application.DTOs.SystemSettings;
using MediatR;

namespace Application.SystemSettings.Queries.GetAll;

public class GetSystemSettingsQueryHandler : IRequestHandler<GetSystemSettingsQuery, SystemSettingsDto>
{
    private readonly ISystemSettingsRepository _repo;

    public GetSystemSettingsQueryHandler(ISystemSettingsRepository repo)
    {
        _repo = repo;
    }

    public async Task<SystemSettingsDto> Handle(GetSystemSettingsQuery request, CancellationToken ct)
    {
        var settings = await _repo.GetAsync(ct);

        return new SystemSettingsDto
        {
            MaxAquariumsPerUser = settings.MaxAquariumsPerUser,
            MaxDevicesPerAquarium = settings.MaxDevicesPerAquarium,
            MaxAlarmRulesPerDevice = settings.MaxAlarmRulesPerDevice,
            MaxScheduledCommandsPerDevice = settings.MaxScheduledCommandsPerDevice
        };
    }
}
