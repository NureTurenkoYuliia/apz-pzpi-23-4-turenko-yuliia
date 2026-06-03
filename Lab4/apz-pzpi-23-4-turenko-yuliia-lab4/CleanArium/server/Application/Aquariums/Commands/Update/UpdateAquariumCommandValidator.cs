using Application.Abstractions;
using FluentValidation;

namespace Application.Aquariums.Commands.Update;

public class UpdateAquariumCommandValidator : AbstractValidator<UpdateAquariumCommand>
{
    private readonly IAquariumRepository _repo;
    private readonly ISystemSettingsRepository _settingsRepo;

    public UpdateAquariumCommandValidator(IAquariumRepository repo, ISystemSettingsRepository settingsRepo)
    {
        _repo = repo;
        _settingsRepo = settingsRepo;

        RuleFor(x => x.AquariumId).NotEmpty().GreaterThan(0)
            .WithMessage("Aquarium id is required to update aquarium.");

        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name is required to update aquarium.")
            .MaximumLength(30).WithMessage("Title max length must not exceed 30 symbols.");

        RuleFor(x => x.Location)
            .MaximumLength(80).WithMessage("Location max length must not exceed 80 symbols.");

        RuleFor(x => x)
            .MustAsync(AquariumExists).WithMessage("User aquarium not found.")
            .MustAsync(ExceedsNumberOfAquariums).WithMessage("Maximum aquariums per parameter was exceeded.");
    }

    private async Task<bool> AquariumExists(UpdateAquariumCommand cmd, CancellationToken ct)
    {
        return await _repo.ExistsByUserIdAsync(cmd.UserId, cmd.AquariumId, ct);
    }

    private async Task<bool> ExceedsNumberOfAquariums(UpdateAquariumCommand cmd, CancellationToken ct)
    {
        var existing = await _repo.CountByUser(cmd.UserId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        return existing >= settings.MaxAquariumsPerUser;
    }
}
