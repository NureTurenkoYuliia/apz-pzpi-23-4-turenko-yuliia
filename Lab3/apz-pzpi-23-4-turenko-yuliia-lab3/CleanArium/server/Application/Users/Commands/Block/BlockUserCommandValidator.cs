using Application.Abstractions;
using FluentValidation;

namespace Application.Users.Commands.Block;

public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
{
    private readonly IUserRepository _userRepo;

    public BlockUserCommandValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to block.");

        RuleFor(x => x)
           .MustAsync(UserExists).WithMessage("User not found.");
    }

    private async Task<bool> UserExists(BlockUserCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }
}
