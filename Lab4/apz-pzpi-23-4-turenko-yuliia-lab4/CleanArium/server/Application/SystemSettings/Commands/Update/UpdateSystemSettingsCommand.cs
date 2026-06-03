using MediatR;

namespace Application.SystemSettings.Commands.Update;

public record UpdateSystemSettingsCommand(
    int MaxAquariumsPerUser,
    int MaxDevicesPerAquarium,
    int MaxAlarmRulesPerDevice,
    int MaxScheduledCommandsPerDevice) : IRequest;
