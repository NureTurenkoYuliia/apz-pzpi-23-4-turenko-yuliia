using Application.Abstractions;
using MediatR;

namespace Application.SystemSettings.Commands.Update;

public class UpdateSystemSettingsCommandHandler : IRequestHandler<UpdateSystemSettingsCommand>
{
    private readonly ISystemSettingsRepository _repo;

    public UpdateSystemSettingsCommandHandler(ISystemSettingsRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(UpdateSystemSettingsCommand request, CancellationToken ct)
    {
        var settings = await _repo.GetAsync(ct);

        settings.MaxAquariumsPerUser = request.MaxAquariumsPerUser;
        settings.MaxDevicesPerAquarium = request.MaxDevicesPerAquarium;
        settings.MaxAlarmRulesPerDevice = request.MaxAlarmRulesPerDevice;
        settings.MaxScheduledCommandsPerDevice = request.MaxScheduledCommandsPerDevice;

        await _repo.UpdateAsync(settings, ct);
    }
}
