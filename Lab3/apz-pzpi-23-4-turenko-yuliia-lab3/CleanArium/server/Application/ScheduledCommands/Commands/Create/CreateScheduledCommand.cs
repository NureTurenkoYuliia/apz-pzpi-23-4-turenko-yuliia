using Domain.Enums;
using MediatR;

namespace Application.ScheduledCommands.Commands.Create;

public record CreateScheduledCommand(
    long UserId,
    long DeviceId,
    CommandType CommandType,
    DateTime StartTime,
    RepeatMode RepeatMode,
    int? IntervalMinutes,
    bool IsActive) : IRequest<long>;