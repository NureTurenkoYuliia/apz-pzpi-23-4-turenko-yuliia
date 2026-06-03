using Application.Abstractions;
using FluentValidation;

namespace Application.AlarmRules.Commands.Create;

public class CreateAlarmRuleCommandValidator : AbstractValidator<CreateAlarmRuleCommand>
{
    private readonly IAlarmRuleRepository _alarmRuleRepo;
    private readonly IDeviceRepository _deviceRepo;
    private readonly ISystemSettingsRepository _settingsRepo;

    public CreateAlarmRuleCommandValidator(
        IAlarmRuleRepository alarmRuleRepo, 
        IDeviceRepository deviceRepo, 
        ISystemSettingsRepository settingsRepo)
    {
        _alarmRuleRepo = alarmRuleRepo;
        _deviceRepo = deviceRepo;
        _settingsRepo = settingsRepo;

        RuleFor(x => x.DeviceId).NotEmpty().GreaterThan(0)
            .WithMessage("Device id is required to create alarm rule.");

        RuleFor(x => x.Condition).NotEmpty()
           .WithMessage("Condition for rule is required.");

        RuleFor(x => x)
            .MustAsync(DeviceExists).WithMessage("Device not found.")
            .MustAsync(UserOwnsDevice).WithMessage("User doesn't own this device.")
            .MustAsync(ExceedsNumberOfRules).WithMessage("Maximum per parameter was exceeded.");
    }

    private async Task<bool> DeviceExists(CreateAlarmRuleCommand cmd, CancellationToken ct)
    {
        return await _deviceRepo.ExistsByIdAsync(cmd.DeviceId, ct);
    }

    private async Task<bool> UserOwnsDevice(CreateAlarmRuleCommand cmd, CancellationToken ct)
    {
        return await _deviceRepo.UserOwnsDeviceAsync(cmd.UserId, cmd.DeviceId, ct);
    }

    private async Task<bool> ExceedsNumberOfRules(CreateAlarmRuleCommand cmd, CancellationToken ct)
    {
        var existing = await _alarmRuleRepo.CountByDevice(cmd.DeviceId,ct);
        var settings = await _settingsRepo.GetAsync(ct);

        return existing < settings.MaxAlarmRulesPerDevice;
    }
}