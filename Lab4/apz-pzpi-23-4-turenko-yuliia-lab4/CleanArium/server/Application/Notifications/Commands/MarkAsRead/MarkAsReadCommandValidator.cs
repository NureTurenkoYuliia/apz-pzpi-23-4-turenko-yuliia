using Application.Abstractions;
using FluentValidation;

namespace Application.Notifications.Commands.MarkAsRead;

public class MarkAsReadCommandValidator : AbstractValidator<MarkAsReadCommand>
{
    private readonly INotificationRepository _repo;
    private readonly IUserRepository _userRepo;

    public MarkAsReadCommandValidator(INotificationRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;

        RuleFor(x => x.NotificationId).NotEmpty().GreaterThan(0)
            .WithMessage("Notification id is required to alarm rule.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.")
            .MustAsync(NotificationExists).WithMessage("Device not found.");
    }

    private async Task<bool> UserExists(MarkAsReadCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }

    private async Task<bool> NotificationExists(MarkAsReadCommand cmd, CancellationToken ct)
    {
        return await _repo.ExistsAsync(cmd.UserId, cmd.NotificationId, ct);
    }
}