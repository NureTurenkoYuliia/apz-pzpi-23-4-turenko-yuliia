using Application.Abstractions;
using Domain.Enums;
using FluentValidation;

namespace Application.SupportMessages.Commands.Delete;

public class DeleteMessagesCommandValidator : AbstractValidator<DeleteMessagesCommand>
{
    private readonly ISupportMessageRepository _repo;
    private readonly IUserRepository _userRepo;

    public DeleteMessagesCommandValidator(IUserRepository userRepo, ISupportMessageRepository repo)
    {
        _repo = repo;
        _userRepo = userRepo;

        RuleFor(x => x.FirstMessageId).NotEmpty()
             .WithMessage("Message id is required to delete history of messages.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.")
            .MustAsync(MessageExists).WithMessage("Message not found.")
            .MustAsync(HasAdminPermission).WithMessage("Only admin can delete all messages.");
    }

    private async Task<bool> UserExists(DeleteMessagesCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }

    private async Task<bool> MessageExists(DeleteMessagesCommand cmd, CancellationToken ct)
    {
        return await _repo.ExistsAsync(cmd.FirstMessageId, ct);
    }

    private async Task<bool> HasAdminPermission(DeleteMessagesCommand cmd, CancellationToken ct)
    {
        UserRole role = await _userRepo.GetUserRoleByIdAsync(cmd.UserId, ct);
        return role == UserRole.Admin;
    }
}
