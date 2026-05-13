using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.Delete;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _repo;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(IUserRepository repo, ILogger<DeleteUserCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId, ct);

        user.IsBlocked = true;
        await _repo.UpdateAsync(user, ct);

        _logger.LogInformation("User {UserId} is deleted by admin", request.UserId);
    }
}