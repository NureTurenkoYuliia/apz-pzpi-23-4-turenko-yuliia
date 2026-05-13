using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.Unblock;

public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand>
{
    private readonly IUserRepository _repo;
    private readonly ILogger<UnblockUserCommandHandler> _logger;

    public UnblockUserCommandHandler(IUserRepository repo, ILogger<UnblockUserCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(UnblockUserCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId, ct);

        user.IsBlocked = false;
        await _repo.UpdateAsync(user, ct);

        _logger.LogInformation("User {UserId} is unblocked now", request.UserId);
    }
}