using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.Block;

public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand>
{
    private readonly IUserRepository _repo;
    private readonly ILogger<BlockUserCommandHandler> _logger;

    public BlockUserCommandHandler(IUserRepository repo, ILogger<BlockUserCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(BlockUserCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId, ct);

        user.IsBlocked = true;
        await _repo.UpdateAsync(user, ct);

        _logger.LogInformation("User {UserId} is blocked now", request.UserId);
    }
}
