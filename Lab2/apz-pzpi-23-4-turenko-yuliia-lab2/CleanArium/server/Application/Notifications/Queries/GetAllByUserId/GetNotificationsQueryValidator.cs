using Application.Abstractions;
using FluentValidation;

namespace Application.Notifications.Queries.GetAllByUserId;

public class GetNotificationsQueryValidator : AbstractValidator<GetNotificationsQuery>
{
    private readonly IUserRepository _userRepo;

    public GetNotificationsQueryValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to get notifications.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.");
    }

    private async Task<bool> UserExists(GetNotificationsQuery cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }
}
