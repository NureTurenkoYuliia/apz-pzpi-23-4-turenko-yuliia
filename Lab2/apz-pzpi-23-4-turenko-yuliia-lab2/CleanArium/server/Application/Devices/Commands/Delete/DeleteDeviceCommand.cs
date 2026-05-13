using MediatR;

namespace Application.Devices.Commands.Delete;

public record DeleteDeviceCommand(long UserId, long DeviceId) : IRequest;