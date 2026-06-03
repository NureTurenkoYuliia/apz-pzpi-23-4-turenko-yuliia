using Application.Abstractions;
using FluentValidation;

namespace Application.Devices.Commands.Create;

public class CreateDeviceCommandValidator : AbstractValidator<CreateDeviceCommand>
{
    private readonly IDeviceRepository _deviceRepo;
    private readonly IAquariumRepository _aquariumRepo;
    private readonly ISystemSettingsRepository _settingsRepo;

    public CreateDeviceCommandValidator(IDeviceRepository deviceRepo, IAquariumRepository aquariumRepo, ISystemSettingsRepository settingsRepo)
    {
        _deviceRepo = deviceRepo;
        _aquariumRepo = aquariumRepo;
        _settingsRepo = settingsRepo;

        RuleFor(x => x.AquariumId).NotEmpty().GreaterThan(0)
            .WithMessage("Aquarium id is required to create device.");

        RuleFor(x => x.DeviceType).NotEmpty()
            .WithMessage("Choose Device Type to create new device.");

        RuleFor(x => x)
            .MustAsync(AquariumExists).WithMessage("Aquarium not found.")
            .MustAsync(ExceedsNumberOfDevices).WithMessage("Maximum aquariums per parameter was exceeded.");
    }

    private async Task<bool> AquariumExists(CreateDeviceCommand cmd, CancellationToken ct)
    {
        return await _aquariumRepo.ExistsByUserIdAsync(cmd.UserId, cmd.AquariumId, ct);
    }

    private async Task<bool> ExceedsNumberOfDevices(CreateDeviceCommand cmd, CancellationToken ct)
    {
        var existing = await _deviceRepo.CountByAquarium(cmd.AquariumId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        return existing < settings.MaxDevicesPerAquarium;
    }
}