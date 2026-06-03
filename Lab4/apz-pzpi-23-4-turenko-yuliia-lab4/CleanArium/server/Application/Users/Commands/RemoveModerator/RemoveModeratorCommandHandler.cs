using Application.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.Users.Commands.RemoveModerator;

public class RemoveModeratorCommandHandler : IRequestHandler<RemoveModeratorCommand>
{
    private readonly IUserRepository _repo;

    public RemoveModeratorCommandHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(RemoveModeratorCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId, ct);

        user.Role = UserRole.User;
        await _repo.UpdateAsync(user, ct);
    }
}