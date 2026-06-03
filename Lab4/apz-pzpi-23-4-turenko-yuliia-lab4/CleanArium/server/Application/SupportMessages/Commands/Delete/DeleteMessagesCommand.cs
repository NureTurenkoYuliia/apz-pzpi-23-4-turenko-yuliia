using MediatR;

namespace Application.SupportMessages.Commands.Delete;

public record DeleteMessagesCommand(long UserId, long FirstMessageId) : IRequest;
