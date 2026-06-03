using Application.Abstractions;
using FluentValidation;

namespace Application.ScheduledCommands.Commands.Update;

public class UpdateScheduledCommandValidator : AbstractValidator<UpdateScheduledCommand>
{
    private readonly IScheduledCommandRepository _scheduledRepo;

    public UpdateScheduledCommandValidator(IScheduledCommandRepository scheduledRepo)
    {
        _scheduledRepo = scheduledRepo;

        RuleFor(x => x.CommandId).NotEmpty().GreaterThan(0)
            .WithMessage("Scheduled Command id is required to update scheduled command.");

        RuleFor(x => x)
            .MustAsync(CommandExists).WithMessage("Scheduled Command not found.");
    }

    private async Task<bool> CommandExists(UpdateScheduledCommand cmd, CancellationToken ct)
    {
        return await _scheduledRepo.ExistsByUserIdAsync(cmd.UserId, cmd.CommandId, ct);
    }
}