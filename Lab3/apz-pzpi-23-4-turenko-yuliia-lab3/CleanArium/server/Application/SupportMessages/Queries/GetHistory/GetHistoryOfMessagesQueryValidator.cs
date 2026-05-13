using Application.Abstractions;
using Domain.Enums;
using FluentValidation;

namespace Application.SupportMessages.Queries.GetHistory;

public class GetHistoryOfMessagesQueryValidator : AbstractValidator<GetHistoryOfMessagesQuery>
{
    private readonly ISupportMessageRepository _repo;
    private readonly IUserRepository _userRepo;

    public GetHistoryOfMessagesQueryValidator(IUserRepository userRepo, ISupportMessageRepository repo)
    {
        _repo = repo;
        _userRepo = userRepo;

        RuleFor(x => x.FirstMessageId).NotEmpty()
             .WithMessage("Message id is required to update.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.")
            .MustAsync(MessageExists).WithMessage("Message not found.")
            .MustAsync(HasPermission).WithMessage("You can't get history of messages message.");
    }

    private async Task<bool> UserExists(GetHistoryOfMessagesQuery cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }

    private async Task<bool> MessageExists(GetHistoryOfMessagesQuery cmd, CancellationToken ct)
    {
        return await _repo.ExistsAsync(cmd.FirstMessageId, ct);
    }

    private async Task<bool> HasPermission(GetHistoryOfMessagesQuery cmd, CancellationToken ct)
    {
        UserRole role = await _userRepo.GetUserRoleByIdAsync(cmd.UserId, ct);

        return await _repo.MessageBelongsToUserAsync(cmd.FirstMessageId, cmd.UserId) || role == UserRole.Admin;
    }
}
