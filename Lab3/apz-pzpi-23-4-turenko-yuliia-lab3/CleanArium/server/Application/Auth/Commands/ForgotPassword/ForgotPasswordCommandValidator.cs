using Application.Abstractions;
using FluentValidation;

namespace Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepo;

    public ForgotPasswordCommandValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.Email).NotEmpty().EmailAddress()
            .WithMessage("Email is required to forgot user password.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.");
    }

    private async Task<bool> UserExists(ForgotPasswordCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByEmailAsync(cmd.Email, ct);
        return user != null;
    }
}
