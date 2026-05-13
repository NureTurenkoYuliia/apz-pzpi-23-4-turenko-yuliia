using Domain.Enums;
using MediatR;

namespace Application.Devices.Commands.Update;

public record UpdateDeviceCommand(long UserId, long DeviceId, DeviceType DeviceType, DeviceStatus DeviceStatus) : IRequest;
