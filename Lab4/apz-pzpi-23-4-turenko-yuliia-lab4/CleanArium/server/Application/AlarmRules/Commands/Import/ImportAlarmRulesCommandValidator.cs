using Application.Abstractions;
using Application.AlarmRules.Commands.Create;
using FluentValidation;

namespace Application.AlarmRules.Commands.Import;

public class ImportAlarmRulesCommandValidator : AbstractValidator<ImportAlarmRulesCommand>
{
    private readonly IAlarmRuleRepository _alarmRuleRepo;
    private readonly IDeviceRepository _deviceRepo;
    private readonly ISystemSettingsRepository _settingsRepo;

    public ImportAlarmRulesCommandValidator(
        IAlarmRuleRepository alarmRuleRepo, 
        IDeviceRepository deviceRepo, 
        ISystemSettingsRepository settingsRepo)
    {
        _alarmRuleRepo = alarmRuleRepo;
        _deviceRepo = deviceRepo;
        _settingsRepo = settingsRepo;


        RuleFor(x => x.DeviceId).NotEmpty().GreaterThan(0)
            .WithMessage("Device id is required to import alarm rules.");

        RuleFor(x => x)
            .MustAsync(DeviceExists).WithMessage("Device not found.")
            .MustAsync(UserOwnsDevice).WithMessage("User doesn't own this device.")
            .MustAsync(ExceedsNumberOfRules).WithMessage("Maximum rules per parameter was exceeded.");
    }

    private async Task<bool> DeviceExists(ImportAlarmRulesCommand cmd, CancellationToken ct)
    {
        return await _deviceRepo.ExistsByIdAsync(cmd.DeviceId, ct);
    }

    private async Task<bool> UserOwnsDevice(ImportAlarmRulesCommand cmd, CancellationToken ct)
    {
        return await _deviceRepo.UserOwnsDeviceAsync(cmd.UserId, cmd.DeviceId, ct);
    }

    private async Task<bool> ExceedsNumberOfRules(ImportAlarmRulesCommand cmd, CancellationToken ct)
    {
        var existing = await _alarmRuleRepo.CountByDevice(cmd.DeviceId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        return existing < settings.MaxAlarmRulesPerDevice;
    }
}