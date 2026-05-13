using Application.Abstractions;
using Application.DTOs.Auth;
using MediatR;

namespace Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUserRepository _userRepo;
    private readonly IJwtTokenService _jwt;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshRepo,
        IUserRepository userRepo,
        IJwtTokenService jwt)
    {
        _refreshRepo = refreshRepo;
        _userRepo = userRepo;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var stored = await _refreshRepo.GetByTokenAsync(request.RefreshToken);
        var user = await _userRepo.GetByIdAsync(stored.UserId, ct);

        stored.IsRevoked = true;
        await _refreshRepo.UpdateAsync(stored);

        var newRefresh = _jwt.GenerateRefreshToken();
        newRefresh.UserId = user.Id;

        await _refreshRepo.AddAsync(newRefresh);

        return new AuthResponseDto
        {
            AccessToken = _jwt.GenerateToken(user),
            RefreshToken = newRefresh.Token
        };
    }
}
