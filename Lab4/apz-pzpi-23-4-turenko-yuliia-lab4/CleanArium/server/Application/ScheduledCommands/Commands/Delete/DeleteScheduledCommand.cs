using MediatR;

namespace Application.ScheduledCommands.Commands.Delete;

public record DeleteScheduledCommand(long UserId, long CommandId) : IRequest;