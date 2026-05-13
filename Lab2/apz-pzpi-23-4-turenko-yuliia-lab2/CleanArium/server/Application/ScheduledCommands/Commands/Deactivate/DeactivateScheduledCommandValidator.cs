using Application.Abstractions;
using Domain.Enums;
using FluentValidation;

namespace Application.ScheduledCommands.Commands.Deactivate;

public class DeactivateScheduledCommandValidator : AbstractValidator<DeactivateScheduledCommand>
{
    private readonly IScheduledCommandRepository _scheduledCommandRepo;
    private readonly IUserRepository _userRepo;

    public DeactivateScheduledCommandValidator(IScheduledCommandRepository scheduledCommandRepo, IUserRepository userRepo)
    {
        _scheduledCommandRepo = scheduledCommandRepo;
        _userRepo = userRepo;

        RuleFor(x => x.CommandId).NotEmpty().GreaterThan(0)
            .WithMessage("Scheduled Command id is required to deactivate scheduled command.");

        RuleFor(x => x)
            .MustAsync(CommandExists).WithMessage("Scheduled Command not found.");
    }
    private async Task<bool> CommandExists(DeactivateScheduledCommand cmd, CancellationToken ct)
    {
        return await _scheduledCommandRepo.ExistsByIdAsync(cmd.CommandId, ct);
    }
}