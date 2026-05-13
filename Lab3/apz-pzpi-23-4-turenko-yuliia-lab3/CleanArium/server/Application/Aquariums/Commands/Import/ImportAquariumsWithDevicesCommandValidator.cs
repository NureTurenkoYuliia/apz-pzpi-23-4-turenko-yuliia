using Application.Abstractions;
using FluentValidation;

namespace Application.Aquariums.Commands.Import;

public class ImportAquariumsWithDevicesCommandValidator : AbstractValidator<ImportAquariumsWithDevicesCommand>
{
    private readonly IAquariumRepository _repo;
    private readonly ISystemSettingsRepository _settingsRepo;

    public ImportAquariumsWithDevicesCommandValidator(IAquariumRepository repo, ISystemSettingsRepository settingsRepo)
    {
        _repo = repo;
        _settingsRepo = settingsRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to import aquariums with devices.");

        RuleFor(x => x)
            .MustAsync(ExceedsNumberOfAquariums).WithMessage("Maximum aquariums per parameter was exceeded.");
    }

    private async Task<bool> ExceedsNumberOfAquariums(ImportAquariumsWithDevicesCommand cmd, CancellationToken ct)
    {
        var existing = await _repo.CountByUser(cmd.UserId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        return existing < settings.MaxAquariumsPerUser;
    }
}