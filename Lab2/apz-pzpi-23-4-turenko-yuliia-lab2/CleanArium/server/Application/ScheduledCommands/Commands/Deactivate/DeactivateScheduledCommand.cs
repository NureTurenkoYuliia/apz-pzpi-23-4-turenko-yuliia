using MediatR;

namespace Application.ScheduledCommands.Commands.Deactivate;

public record DeactivateScheduledCommand(long CommandId) : IRequest;
