using Application.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.Users.Commands.AssignModerator;

public class AssignModeratorCommandHandler : IRequestHandler<AssignModeratorCommand>
{
    private readonly IUserRepository _repo;

    public AssignModeratorCommandHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(AssignModeratorCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId, ct);

        user.Role = UserRole.Moderator;
        await _repo.UpdateAsync(user, ct);
    }
}
