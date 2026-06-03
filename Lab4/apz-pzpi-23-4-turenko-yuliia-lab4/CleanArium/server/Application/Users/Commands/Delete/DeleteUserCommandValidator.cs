using Application.Abstractions;
using FluentValidation;

namespace Application.Users.Commands.Delete;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly IUserRepository _userRepo;

    public DeleteUserCommandValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to block.");

        RuleFor(x => x)
           .MustAsync(UserExists).WithMessage("User not found.");
    }

    private async Task<bool> UserExists(DeleteUserCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }
}
