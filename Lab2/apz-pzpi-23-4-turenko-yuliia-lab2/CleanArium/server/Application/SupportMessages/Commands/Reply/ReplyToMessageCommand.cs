using MediatR;

namespace Application.SupportMessages.Commands.Reply;

public record ReplyToMessageCommand(
    long UserId, 
    long MessageId, 
    string Subject,
    string Message) : IRequest<long>;
