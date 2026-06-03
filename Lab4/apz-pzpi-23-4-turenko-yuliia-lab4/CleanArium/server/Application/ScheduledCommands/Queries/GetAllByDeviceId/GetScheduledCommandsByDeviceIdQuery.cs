using Application.DTOs.ScheduledCommands;
using MediatR;

namespace Application.ScheduledCommands.Queries.GetAllByDeviceId;

public record GetScheduledCommandsByDeviceIdQuery (long UserId, long DeviceId) : IRequest<List<ScheduledCommandDto>>;
