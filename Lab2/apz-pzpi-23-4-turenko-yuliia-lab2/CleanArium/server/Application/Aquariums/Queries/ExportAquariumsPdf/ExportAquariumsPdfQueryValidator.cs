using Application.Abstractions;
using FluentValidation;

namespace Application.Aquariums.Queries.ExportAquariumsPdf;

public class ExportAquariumsPdfQueryValidator : AbstractValidator<ExportAquariumsPdfQuery>
{
    private readonly IUserRepository _userRepo;

    public ExportAquariumsPdfQueryValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to export data about aquariums.");

        RuleFor(x => x)
           .MustAsync(UserExists).WithMessage("User not found.");
    }

    private async Task<bool> UserExists(ExportAquariumsPdfQuery cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }
}