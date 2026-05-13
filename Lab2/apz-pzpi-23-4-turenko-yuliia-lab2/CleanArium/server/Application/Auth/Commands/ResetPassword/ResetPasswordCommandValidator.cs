using Application.Abstractions;
using FluentValidation;

namespace Application.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordResetTokenRepository _tokenRepo;

    public ResetPasswordCommandValidator(IUserRepository userRepo, IPasswordResetTokenRepository tokenRepo)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;

        RuleFor(x => x.Token).NotEmpty()
            .WithMessage("Token is required to reset user password.");

        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8)
            .WithMessage("Password is required to reset user password.");

        RuleFor(x => x)
           .MustAsync(UserWithTokenExists).WithMessage("Invalid token or User not found.");
    }

    private async Task<bool> UserWithTokenExists(ResetPasswordCommand cmd, CancellationToken ct)
    {
        var token = await _tokenRepo.GetByTokenAsync(cmd.Token, ct);

        if (token == null || token.IsUsed || token.ExpiresAt < DateTime.UtcNow)
            return false;

        var user = await _userRepo.GetByIdAsync(token.UserId, ct);
        return user != null;
    }
}
