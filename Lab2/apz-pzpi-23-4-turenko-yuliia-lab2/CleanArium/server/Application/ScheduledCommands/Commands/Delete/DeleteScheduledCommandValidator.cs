using Application.Abstractions;
using FluentValidation;

namespace Application.ScheduledCommands.Commands.Delete;

public class DeleteScheduledCommandValidator : AbstractValidator<DeleteScheduledCommand>
{
    private readonly IScheduledCommandRepository _scheduledCommandRepo;
    private readonly IUserRepository _userRepo;

    public DeleteScheduledCommandValidator(IScheduledCommandRepository scheduledCommandRepo, IUserRepository userRepo)
    {
        _scheduledCommandRepo = scheduledCommandRepo;
        _userRepo = userRepo;

        RuleFor(x => x.CommandId).NotEmpty().GreaterThan(0)
            .WithMessage("Scheduled Command id is required to update scheduled command.");



        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.")
            .MustAsync(CommandExists).WithMessage("Scheduled Command not found.");
    }

    private async Task<bool> UserExists(DeleteScheduledCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }

    private async Task<bool> CommandExists(DeleteScheduledCommand cmd, CancellationToken ct)
    {
        return await _scheduledCommandRepo.ExistsByUserIdAsync(cmd.UserId, cmd.CommandId, ct);
    }
}