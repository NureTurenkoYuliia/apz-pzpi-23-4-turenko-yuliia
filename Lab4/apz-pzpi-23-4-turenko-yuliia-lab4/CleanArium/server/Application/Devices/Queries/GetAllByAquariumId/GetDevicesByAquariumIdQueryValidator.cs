using Application.Abstractions;
using FluentValidation;

namespace Application.Devices.Queries.GetAllByAquariumId;

public class GetDevicesByAquariumIdQueryValidator : AbstractValidator<GetDevicesByAquariumIdQuery>
{
    private readonly IAquariumRepository _repo;

    public GetDevicesByAquariumIdQueryValidator(IAquariumRepository repo)
    {
        _repo = repo;
        
        RuleFor(x => x.AquariumId).NotEmpty().GreaterThan(0)
            .WithMessage("Aquarium id is required to get its devices.");

        RuleFor(x => x)
            .MustAsync(AquariumExists).WithMessage("Aquarium not found.");
    }

    private async Task<bool> AquariumExists(GetDevicesByAquariumIdQuery cmd, CancellationToken ct)
    {
        return await _repo.ExistsByUserIdAsync(cmd.UserId, cmd.AquariumId, ct);
    }
}
