using Application.Abstractions;
using FluentValidation;

namespace Application.Aquariums.Queries.GetAllByUserId;

public class GetAquariumsByUserIdQueryValidator : AbstractValidator<GetAquariumsByUserIdQuery>
{
    private readonly IUserRepository _userRepo;

    public GetAquariumsByUserIdQueryValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to get aquariums.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.");
    }

    private async Task<bool> UserExists(GetAquariumsByUserIdQuery cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }
}
