using MediatR;

namespace Application.Users.Commands.RemoveModerator;

public record RemoveModeratorCommand(long UserId) : IRequest;