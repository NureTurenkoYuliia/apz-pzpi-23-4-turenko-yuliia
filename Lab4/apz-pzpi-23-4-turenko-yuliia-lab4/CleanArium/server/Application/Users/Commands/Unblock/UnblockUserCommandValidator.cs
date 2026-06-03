using Application.Abstractions;
using FluentValidation;

namespace Application.Users.Commands.Unblock;

public class UnblockUserCommandValidator : AbstractValidator<UnblockUserCommand>
{
    private readonly IUserRepository _userRepo;

    public UnblockUserCommandValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to unblock.");

        RuleFor(x => x)
           .MustAsync(UserExists).WithMessage("User not found.");
    }

    private async Task<bool> UserExists(UnblockUserCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }
}
