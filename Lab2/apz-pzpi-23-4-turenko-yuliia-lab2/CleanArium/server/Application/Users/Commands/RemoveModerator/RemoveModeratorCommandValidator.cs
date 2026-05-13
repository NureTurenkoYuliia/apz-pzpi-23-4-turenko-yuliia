using Application.Abstractions;
using Domain.Enums;
using FluentValidation;

namespace Application.Users.Commands.RemoveModerator;

public class RemoveModeratorCommandValidator : AbstractValidator<RemoveModeratorCommand>
{
    private readonly IUserRepository _userRepo;

    public RemoveModeratorCommandValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to remove moderator.");

        RuleFor(x => x)
           .MustAsync(UserExists).WithMessage("User not found.")
           .MustAsync(IsUserModerator).WithMessage("User is not a moderator.");
    }

    private async Task<bool> UserExists(RemoveModeratorCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }

    private async Task<bool> IsUserModerator(RemoveModeratorCommand cmd, CancellationToken ct)
    {
        UserRole role = await _userRepo.GetUserRoleByIdAsync(cmd.UserId, ct);
        return role == UserRole.Moderator;
    }
}
