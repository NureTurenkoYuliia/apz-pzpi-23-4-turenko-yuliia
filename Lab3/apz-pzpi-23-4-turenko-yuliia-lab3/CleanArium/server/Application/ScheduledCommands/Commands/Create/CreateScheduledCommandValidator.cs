using Application.Abstractions;
using Domain.Enums;
using FluentValidation;

namespace Application.ScheduledCommands.Commands.Create;

public class CreateScheduledCommandValidator : AbstractValidator<CreateScheduledCommand>
{
    private readonly IScheduledCommandRepository _scheduledRepo;
    private readonly IDeviceRepository _deviceRepo;
    private readonly ISystemSettingsRepository _settingsRepo;

    public CreateScheduledCommandValidator(
        IScheduledCommandRepository scheduledRepo,
        IDeviceRepository deviceRepo,
        ISystemSettingsRepository settingsRepo)
    {
        _scheduledRepo = scheduledRepo;
        _deviceRepo = deviceRepo;
        _settingsRepo = settingsRepo;

        RuleFor(x => x.DeviceId).NotEmpty().GreaterThan(0)
            .WithMessage("Device id is required to create scheduled command.");

        RuleFor(x => x.CommandType).NotEmpty()
             .WithMessage("Command type is required to create scheduled command.");

        RuleFor(x => x.StartTime).NotEmpty()
             .WithMessage("Start time is required to create scheduled command.");

        RuleFor(x => x.IntervalMinutes).GreaterThan(0)
            .When(x => x.RepeatMode != RepeatMode.None);

        RuleFor(x => x)
            .MustAsync(DeviceExists).WithMessage("Device not found.")
            .MustAsync(UserOwnsDevice).WithMessage("User doesn't own this device.")
            .MustAsync(ExceedsNumberOfCommands).WithMessage("Maximum commands per parameter was exceeded.");
    }

    private async Task<bool> DeviceExists(CreateScheduledCommand cmd, CancellationToken ct)
    {
        return await _deviceRepo.ExistsByIdAsync(cmd.DeviceId, ct);
    }

    private async Task<bool> UserOwnsDevice(CreateScheduledCommand cmd, CancellationToken ct)
    {
        return await _deviceRepo.UserOwnsDeviceAsync(cmd.UserId, cmd.DeviceId, ct);
    }

    private async Task<bool> ExceedsNumberOfCommands(CreateScheduledCommand cmd, CancellationToken ct)
    {
        var existing = await _scheduledRepo.CountByDevice(cmd.DeviceId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        return existing < settings.MaxScheduledCommandsPerDevice;
    }
}