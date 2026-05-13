using Application.Abstractions;
using Domain.Enums;
using FluentValidation;

namespace Application.Users.Commands.AssignModerator;

public class AssignModeratorCommandValidator : AbstractValidator<AssignModeratorCommand>
{
    private readonly IUserRepository _userRepo;

    public AssignModeratorCommandValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to assign moderator.");

        RuleFor(x => x)
           .MustAsync(UserExists).WithMessage("User not found.")
           .MustAsync(IsUserNotModerator).WithMessage("User is already a moderator.");
    }

    private async Task<bool> UserExists(AssignModeratorCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }

    private async Task<bool> IsUserNotModerator(AssignModeratorCommand cmd, CancellationToken ct)
    {
        UserRole role = await _userRepo.GetUserRoleByIdAsync(cmd.UserId, ct);
        return role != UserRole.Moderator;
    }
}
