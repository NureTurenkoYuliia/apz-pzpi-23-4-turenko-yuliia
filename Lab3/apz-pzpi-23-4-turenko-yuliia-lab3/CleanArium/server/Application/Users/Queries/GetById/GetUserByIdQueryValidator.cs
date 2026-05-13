using Application.Abstractions;
using FluentValidation;

namespace Application.Users.Queries.GetById;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    private readonly IUserRepository _userRepo;

    public GetUserByIdQueryValidator(IUserRepository userRepo)
    {
        _userRepo = userRepo;

        RuleFor(x => x.UserId).NotEmpty().GreaterThan(0)
            .WithMessage("User id is required to get user.");

        RuleFor(x => x)
            .MustAsync(UserExists).WithMessage("User not found.");
    }

    private async Task<bool> UserExists(GetUserByIdQuery cmd, CancellationToken ct)
    {
        return await _userRepo.ExistsByIdAsync(cmd.UserId, ct);
    }
}