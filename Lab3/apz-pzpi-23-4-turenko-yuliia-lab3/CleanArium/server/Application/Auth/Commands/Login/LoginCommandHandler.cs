using Application.Abstractions;
using Application.DTOs.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenRepository _refresh;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IUserRepository repo, 
        IPasswordHasher hasher, 
        IJwtTokenService jwt,
        IRefreshTokenRepository refresh,
        ILogger<LoginCommandHandler> logger)
    {
        _repo = repo;
        _hasher = hasher;
        _jwt = jwt;
        _refresh = refresh;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByEmailAsync(request.Email, ct);
        var hash = _hasher.HashPassword(request.Password, user.Salt);

        if (hash != user.PasswordHash)
            throw new Exception("Invalid email or password");

        var accessToken = _jwt.GenerateToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();
        refreshToken.UserId = user.Id;

        await _refresh.AddAsync(refreshToken);

        _logger.LogInformation("Successful login for user: {Id} ", user.Id);

        user.LastLoginAt = DateTime.UtcNow;
        await _repo.UpdateAsync(user, ct);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}
