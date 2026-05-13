using Domain.Enums;
using MediatR;

namespace Application.Devices.Commands.Create;

public record CreateDeviceCommand(long UserId, long AquariumId, DeviceType DeviceType, DeviceStatus DeviceStatus) : IRequest<long>;
