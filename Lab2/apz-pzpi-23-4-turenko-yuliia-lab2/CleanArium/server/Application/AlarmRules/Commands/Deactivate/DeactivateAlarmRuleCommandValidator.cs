using Application.Abstractions;
using FluentValidation;

namespace Application.AlarmRules.Commands.Deactivate;

public class DeactivateAlarmRuleCommandValidator : AbstractValidator<DeactivateAlarmRuleCommand>
{
    private readonly IAlarmRuleRepository _alarmRuleRepo;

    public DeactivateAlarmRuleCommandValidator(IAlarmRuleRepository alarmRuleRepo)
    {
        _alarmRuleRepo = alarmRuleRepo;

        RuleFor(x => x.RuleId).NotEmpty().GreaterThan(0)
            .WithMessage("Rule id is required to deactivate alarm rule.");

        RuleFor(x => x)
            .MustAsync(RuleExists).WithMessage("Rule not found.");  
    }

    private async Task<bool> RuleExists(DeactivateAlarmRuleCommand cmd, CancellationToken ct)
    {
        return await _alarmRuleRepo.ExistsByIdAsync(cmd.RuleId, ct);
    }
}
