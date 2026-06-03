using Application.Abstractions;
using FluentValidation;

namespace Application.ScheduledCommands.Commands.Import;

public class ImportScheduledCommandsCommandValidator : AbstractValidator<ImportScheduledCommandsCommand>
{
    private readonly IScheduledCommandRepository _scheduledRepo;
    private readonly IDeviceRepository _deviceRepo;
    private readonly ISystemSettingsRepository _settingsRepo;

    public ImportScheduledCommandsCommandValidator(
        IScheduledCommandRepository scheduledRepo,
        IDeviceRepository deviceRepo,
        ISystemSettingsRepository settingsRepo)
    {
        _scheduledRepo = scheduledRepo;
        _deviceRepo = deviceRepo;
        _settingsRepo = settingsRepo;

        RuleFor(x => x.DeviceId).NotEmpty().GreaterThan(0)
            .WithMessage("Device id is required to import scheduled commands.");

        RuleFor(x => x)
            .MustAsync(DeviceExists).WithMessage("Device not found.")
            .MustAsync(UserOwnsDevice).WithMessage("User doesn't own this device.")
            .MustAsync(ExceedsNumberOfCommands).WithMessage("Maximum commands per parameter was exceeded.");
    }

    private async Task<bool> DeviceExists(ImportScheduledCommandsCommand cmd, CancellationToken ct)
    {
        return await _deviceRepo.ExistsByIdAsync(cmd.DeviceId, ct);
    }

    private async Task<bool> UserOwnsDevice(ImportScheduledCommandsCommand cmd, CancellationToken ct)
    {
        return await _deviceRepo.UserOwnsDeviceAsync(cmd.UserId, cmd.DeviceId, ct);
    }

    private async Task<bool> ExceedsNumberOfCommands(ImportScheduledCommandsCommand cmd, CancellationToken ct)
    {
        var existing = await _scheduledRepo.CountByDevice(cmd.DeviceId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        return existing < settings.MaxScheduledCommandsPerDevice;
    }
}