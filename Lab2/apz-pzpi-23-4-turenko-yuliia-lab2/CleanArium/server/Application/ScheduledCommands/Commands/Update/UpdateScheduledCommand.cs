using Domain.Enums;
using MediatR;

namespace Application.ScheduledCommands.Commands.Update;

public record UpdateScheduledCommand(
    long UserId,
    long CommandId,
    CommandType CommandType,
    DateTime StartTime,
    RepeatMode RepeatMode,
    int? IntervalMinutes,
    bool IsActive) : IRequest;