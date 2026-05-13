using Application.Abstractions;
using FluentValidation;

namespace Application.Aquariums.Commands.Create;

public class CreateAquariumCommandValidator : AbstractValidator<CreateAquariumCommand>
{
    private readonly IAquariumRepository _repo;
    private readonly ISystemSettingsRepository _settingsRepo;

    public CreateAquariumCommandValidator(IAquariumRepository repo, ISystemSettingsRepository settingsRepo)
    {
        _repo = repo;
        _settingsRepo = settingsRepo;

        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name is required to create aquarium.")
            .MaximumLength(30).WithMessage("Title max length must not exceed 30 symbols.");

        RuleFor(x => x.Location)
            .MaximumLength(80).WithMessage("Location max length must not exceed 80 symbols.");

        RuleFor(x => x)
            .MustAsync(AquariumWithSameNameExists).WithMessage("Aquarium with such name already exists.")
            .MustAsync(ExceedsNumberOfAquariums).WithMessage("Maximum aquariums per parameter was exceeded.");
    }

    private async Task<bool> AquariumWithSameNameExists(CreateAquariumCommand cmd, CancellationToken ct)
    {
        return !await _repo.ExistsByNameAsync(cmd.UserId, cmd.Name, ct);
    }

    private async Task<bool> ExceedsNumberOfAquariums(CreateAquariumCommand cmd, CancellationToken ct)
    {
        var existing = await _repo.CountByUser(cmd.UserId, ct);
        var settings = await _settingsRepo.GetAsync(ct);

        return existing < settings.MaxAquariumsPerUser;
    }
}
