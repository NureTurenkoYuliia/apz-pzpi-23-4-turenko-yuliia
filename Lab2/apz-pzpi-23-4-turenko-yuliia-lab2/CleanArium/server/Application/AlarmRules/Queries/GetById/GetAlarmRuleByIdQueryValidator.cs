using Application.Abstractions;
using FluentValidation;

namespace Application.AlarmRules.Queries.GetById;

public class GetAlarmRuleByIdQueryValidator : AbstractValidator<GetAlarmRuleByIdQuery>
{
    private readonly IAlarmRuleRepository _alarmRuleRepo;

    public GetAlarmRuleByIdQueryValidator(IAlarmRuleRepository alarmRuleRepo)
    {
        _alarmRuleRepo = alarmRuleRepo;

        RuleFor(x => x.RuleId).NotEmpty().GreaterThan(0)
            .WithMessage("Rule id is required to get an alarm rule.");

        RuleFor(x => x)
            .MustAsync(RuleExists).WithMessage("Rule not found.");    
    }

    private async Task<bool> RuleExists(GetAlarmRuleByIdQuery cmd, CancellationToken ct)
    {
        return await _alarmRuleRepo.ExistsByIdAsync(cmd.RuleId, ct);
    }
}
