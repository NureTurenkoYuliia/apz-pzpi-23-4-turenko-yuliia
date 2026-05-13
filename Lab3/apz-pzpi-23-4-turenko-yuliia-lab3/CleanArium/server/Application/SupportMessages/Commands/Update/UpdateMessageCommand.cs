using MediatR;

namespace Application.SupportMessages.Commands.Update;

public record UpdateMessageCommand(
    long UserId, 
    long MessageId, 
    string Subject,
    string Message) : IRequest;