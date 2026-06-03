using Application.Abstractions;
using FluentValidation;

namespace Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUserRepository _userRepo;

    public RefreshTokenCommandValidator(IRefreshTokenRepository refreshRepo, IUserRepository userRepo)
    {
        _refreshRepo = refreshRepo;
        _userRepo = userRepo;

        RuleFor(x => x.RefreshToken).NotEmpty()
            .WithMessage("RefreshToken is required to forgot user password.");

        RuleFor(x => x)
            .MustAsync(UserTokenExists).WithMessage("Invalid refresh token or User not found.");
    }

    private async Task<bool> UserTokenExists(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var stored = await _refreshRepo.GetByTokenAsync(cmd.RefreshToken);

        if (stored == null || stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
            return false;

        var user = await _userRepo.GetByIdAsync(stored.UserId, ct);

        return user != null;
    }
}

