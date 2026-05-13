using Application.DTOs.ExecutedCommands;
using MediatR;

namespace Application.ExecutedCommands.Queries.GetAllByDeviceId;

public record GetExecutedCommandsByDeviceIdQuery(long DeviceId) : IRequest<List<ExecutedCommandDto>>;