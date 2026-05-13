using Application.Abstractions;
using FluentValidation;

namespace Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IUserRepository _userRepo;

    public RegisterCommandValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserName).NotEmpty().MinimumLength(2).MaximumLength(30)
            .WithMessage("User name is required for registration. It should be at least 2 and maximum 30 symbols.");

        RuleFor(x => x.Email).NotEmpty().EmailAddress()
            .WithMessage("Email is required for registration.");

        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);

        RuleFor(x => x)
            .MustAsync(AlreadyExists).WithMessage("User with such email already exists.");
    }

    private async Task<bool> AlreadyExists(RegisterCommand cmd, CancellationToken ct)
    {
        var existing = await _userRepo.GetByEmailAsync(cmd.Email, ct);
        return existing == null;
    }
}
