using Application.Abstractions;
using FluentValidation;

namespace Application.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private readonly IUserRepository _userRepo;

    public LoginCommandValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.Email).NotEmpty().EmailAddress()
            .WithMessage("Email is required to forgot user password.");

        RuleFor(x => x.Password).NotEmpty()
            .WithMessage("Password is required to forgot user password.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.");
    }

    private async Task<bool> UserExists(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByEmailAsync(cmd.Email, ct);
        return user != null;
    }
}
