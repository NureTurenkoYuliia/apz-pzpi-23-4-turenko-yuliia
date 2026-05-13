using Application.Abstractions;
using FluentValidation;

namespace Application.Devices.Commands.Update;

public class UpdateDeviceCommandValidator : AbstractValidator<UpdateDeviceCommand>
{
    private readonly IDeviceRepository _repo;
    private readonly IUserRepository _userRepo;

    public UpdateDeviceCommandValidator(IDeviceRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;

        RuleFor(x => x.DeviceId).NotEmpty().GreaterThan(0)
            .WithMessage("Device id is required to update device.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.")
            .MustAsync(DeviceExists).WithMessage("Device not found.")
            .MustAsync(UserOwnsDevice).WithMessage("User doesn't own this device.");
    }

    private async Task<bool> UserExists(UpdateDeviceCommand cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }

    private async Task<bool> DeviceExists(UpdateDeviceCommand cmd, CancellationToken ct)
    {
        return await _repo.ExistsByIdAsync(cmd.DeviceId, ct);
    }

    private async Task<bool> UserOwnsDevice(UpdateDeviceCommand cmd, CancellationToken ct)
    {
        return await _repo.UserOwnsDeviceAsync(cmd.UserId, cmd.DeviceId, ct);
    }
}