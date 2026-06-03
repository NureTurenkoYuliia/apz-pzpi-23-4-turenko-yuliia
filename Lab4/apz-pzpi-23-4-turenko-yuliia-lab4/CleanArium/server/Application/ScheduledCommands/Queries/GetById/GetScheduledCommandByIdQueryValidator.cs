using Application.Abstractions;
using FluentValidation;

namespace Application.ScheduledCommands.Queries.GetById;

public class GetScheduledCommandByIdQueryValidator : AbstractValidator<GetScheduledCommandByIdQuery>
{
    private readonly IScheduledCommandRepository _scheduledCommandRepo;

    public GetScheduledCommandByIdQueryValidator(IScheduledCommandRepository scheduledCommandRepo)
    {
        _scheduledCommandRepo = scheduledCommandRepo;

        RuleFor(x => x.CommandId).NotEmpty().GreaterThan(0)
            .WithMessage("Scheduled Command id is required to retrieve.");

        RuleFor(x => x)
            .MustAsync(CommandExists).WithMessage("Scheduled Command not found.");
    }

    private async Task<bool> CommandExists(GetScheduledCommandByIdQuery cmd, CancellationToken ct)
    {
        return await _scheduledCommandRepo.ExistsByIdAsync(cmd.CommandId, ct);
    }
}