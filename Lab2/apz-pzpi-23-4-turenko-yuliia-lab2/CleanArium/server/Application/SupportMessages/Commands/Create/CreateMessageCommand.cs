using MediatR;

namespace Application.SupportMessages.Commands.Create;

public record CreateMessageCommand(
    long UserId,
    string Subject,
    string Message) : IRequest<long>;