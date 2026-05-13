using MediatR;

namespace Application.Users.Commands.Block;

public record BlockUserCommand(long UserId) : IRequest;

