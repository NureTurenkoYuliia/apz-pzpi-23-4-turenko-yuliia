using Application.Abstractions;
using FluentValidation;

namespace Application.ScheduledCommands.Queries.GetAllByDeviceId;

public class GetScheduledCommandsByDeviceIdQueryValidator : AbstractValidator<GetScheduledCommandsByDeviceIdQuery>
{
    private readonly IDeviceRepository _deviceRepo;

    public GetScheduledCommandsByDeviceIdQueryValidator(IDeviceRepository deviceRepo)
    {
        _deviceRepo = deviceRepo;

        RuleFor(x => x.DeviceId).NotEmpty().GreaterThan(0)
            .WithMessage("Device id is required to get scheduled commands.");

        RuleFor(x => x)
            .MustAsync(DeviceExists).WithMessage("Device not found.")
            .MustAsync(UserOwnsDevice).WithMessage("User doesn't own this device.");
    }

    private async Task<bool> DeviceExists(GetScheduledCommandsByDeviceIdQuery cmd, CancellationToken ct)
    {
        return await _deviceRepo.ExistsByIdAsync(cmd.DeviceId, ct);
    }

    private async Task<bool> UserOwnsDevice(GetScheduledCommandsByDeviceIdQuery cmd, CancellationToken ct)
    {
        return await _deviceRepo.UserOwnsDeviceAsync(cmd.UserId, cmd.DeviceId, ct);
    }
}