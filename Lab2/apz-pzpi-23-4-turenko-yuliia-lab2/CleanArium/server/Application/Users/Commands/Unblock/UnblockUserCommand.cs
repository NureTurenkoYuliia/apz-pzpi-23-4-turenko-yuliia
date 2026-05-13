using MediatR;

namespace Application.Users.Commands.Unblock;

public record UnblockUserCommand(long UserId) : IRequest;