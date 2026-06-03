using Application.Abstractions;
using MediatR;

namespace Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _repo;

    public LogoutCommandHandler(IRefreshTokenRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var token = await _repo.GetByTokenAsync(request.RefreshToken);
        if (token == null)
            return;

        await _repo.DeleteAsync(token);
    }
}
