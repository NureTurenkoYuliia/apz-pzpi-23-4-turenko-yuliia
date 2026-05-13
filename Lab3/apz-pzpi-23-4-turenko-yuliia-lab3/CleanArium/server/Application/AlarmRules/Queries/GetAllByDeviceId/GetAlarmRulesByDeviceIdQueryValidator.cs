using Application.Abstractions;
using FluentValidation;

namespace Application.AlarmRules.Queries.GetAllByDeviceId;

public class GetAlarmRulesByDeviceIdQueryValidator : AbstractValidator<GetAlarmRulesByDeviceIdQuery>
{
    private readonly IDeviceRepository _deviceRepo;

    public GetAlarmRulesByDeviceIdQueryValidator(IDeviceRepository deviceRepo)
    {
        _deviceRepo = deviceRepo;

        RuleFor(x => x.DeviceId).NotEmpty().GreaterThan(0)
            .WithMessage("Device id is required to alarm rule.");

        RuleFor(x => x)
            .MustAsync(DeviceExists).WithMessage("Device not found.")
            .MustAsync(UserOwnsDevice).WithMessage("User doesn't own this device.");
    }

    private async Task<bool> DeviceExists(GetAlarmRulesByDeviceIdQuery cmd, CancellationToken ct)
    {
        return await _deviceRepo.ExistsByIdAsync(cmd.DeviceId, ct);
    }

    private async Task<bool> UserOwnsDevice(GetAlarmRulesByDeviceIdQuery cmd, CancellationToken ct)
    {
        return await _deviceRepo.UserOwnsDeviceAsync(cmd.UserId, cmd.DeviceId, ct);
    }
}