using Application.Abstractions;
using FluentValidation;

namespace Application.AlarmRules.Commands.Delete;

public class DeleteAlarmRuleCommandValidator : AbstractValidator<DeleteAlarmRuleCommand>
{
    private readonly IAlarmRuleRepository _alarmRuleRepo;
    private readonly IUserRepository _userRepo;

    public DeleteAlarmRuleCommandValidator(IAlarmRuleRepository alarmRuleRepo, IUserRepository userRepo)
    {
        _alarmRuleRepo = alarmRuleRepo;
        _userRepo = userRepo;

        RuleFor(x => x.RuleId).NotEmpty().GreaterThan(0)
            .WithMessage("Rule id is required to alarm rule.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.")
            .MustAsync(RuleExists).WithMessage("Rule not found.");
    }

    private async Task<bool> UserExists(DeleteAlarmRuleCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }

    private async Task<bool> RuleExists(DeleteAlarmRuleCommand cmd, CancellationToken ct)
    {
        return await _alarmRuleRepo.ExistsByUserIdAsync(cmd.UserId, cmd.RuleId, ct);
    }
}