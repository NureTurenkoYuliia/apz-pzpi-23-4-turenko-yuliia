using MediatR;

namespace Application.Users.Commands.AssignModerator;

public record AssignModeratorCommand(long UserId) : IRequest;