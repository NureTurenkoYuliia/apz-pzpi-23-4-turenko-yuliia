using Application.Abstractions;
using FluentValidation;

namespace Application.AlarmRules.Commands.Update;

public class UpdateAlarmRuleCommandValidator : AbstractValidator<UpdateAlarmRuleCommand>
{
    private readonly IAlarmRuleRepository _alarmRuleRepo;
    private readonly ISystemSettingsRepository _settingsRepo;

    public UpdateAlarmRuleCommandValidator(IAlarmRuleRepository alarmRuleRepo, ISystemSettingsRepository settingsRepo)
    {
        _alarmRuleRepo = alarmRuleRepo;
        _settingsRepo = settingsRepo;

        RuleFor(x => x.RuleId).NotEmpty().GreaterThan(0)
            .WithMessage("Rule id is required to update alarm rule.");

        RuleFor(x => x.Condition).NotEmpty()
           .WithMessage("Condition for rule is required.");

        RuleFor(x => x)
            .MustAsync(RuleExists).WithMessage("Rule not found.")
            .MustAsync(ExceedsNumberOfRules).WithMessage("Maximum per parameter was exceeded.");
    }

    private async Task<bool> RuleExists(UpdateAlarmRuleCommand cmd, CancellationToken ct)
    {
        return await _alarmRuleRepo.ExistsByUserIdAsync(cmd.UserId, cmd.RuleId, ct);
    }

    private async Task<bool> ExceedsNumberOfRules(UpdateAlarmRuleCommand cmd, CancellationToken ct)
    {
        var rule = await _alarmRuleRepo.GetByIdAsync(cmd.RuleId, ct);
        var existing = await _alarmRuleRepo.CountByDevice(rule.DeviceId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        return existing < settings.MaxAlarmRulesPerDevice;
    }
}