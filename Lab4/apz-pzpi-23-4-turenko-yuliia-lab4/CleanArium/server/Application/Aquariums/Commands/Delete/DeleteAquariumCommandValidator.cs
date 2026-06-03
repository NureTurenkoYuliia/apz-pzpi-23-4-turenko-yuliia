using Application.Abstractions;
using FluentValidation;

namespace Application.Aquariums.Commands.Delete;

public class DeleteAquariumCommandValidator : AbstractValidator<DeleteAquariumCommand>
{
    private readonly IAquariumRepository _repo;

    public DeleteAquariumCommandValidator(IAquariumRepository repo)
    {
        _repo = repo;

        RuleFor(x => x.AquariumId).NotEmpty().GreaterThan(0)
            .WithMessage("Aquarium id is required to delete aquarium.");

        RuleFor(x => x)
            .MustAsync(AquariumExists).WithMessage("Aquarium not found.");
    }

    private async Task<bool> AquariumExists(DeleteAquariumCommand cmd, CancellationToken ct)
    {
        return await _repo.ExistsByUserIdAsync(cmd.UserId, cmd.AquariumId, ct);
    }
}
