using MediatR;

namespace Application.Users.Commands.Delete;

public record DeleteUserCommand(int UserId) : IRequest;
