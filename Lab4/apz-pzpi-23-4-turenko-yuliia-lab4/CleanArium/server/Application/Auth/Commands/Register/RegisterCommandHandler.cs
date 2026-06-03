using Application.Abstractions;
using Application.DTOs.Auth;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenRepository _refresh;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(IUserRepository repo, 
        IPasswordHasher hasher, 
        IJwtTokenService jwt, 
        IRefreshTokenRepository refresh,
        ILogger<RegisterCommandHandler> logger)
    {
        _repo = repo;
        _hasher = hasher;
        _jwt = jwt;
        _refresh = refresh;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existing = await _repo.GetByEmailAsync(request.Email, ct);
        var salt = _hasher.GenerateSalt();
        var hash = _hasher.HashPassword(request.Password, salt);

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = hash,
            Salt = salt,
            Role = UserRole.User,
            IsBlocked = false
        };

        await _repo.AddAsync(user, ct);

        var accessToken = _jwt.GenerateToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();
        refreshToken.UserId = user.Id;
        await _refresh.AddAsync(refreshToken);

        _logger.LogInformation("Successful registration for user: {Id} ", user.Id);

        return new AuthResponseDto { 
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}
