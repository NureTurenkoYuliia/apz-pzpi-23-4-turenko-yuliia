using Application.Abstractions;
using Application.DTOs.Users;
using MediatR;

namespace Application.Users.Queries.GetModerators;

public class GetModeratorsQueryHandler : IRequestHandler<GetModeratorsQuery, List<ModeratorDto>>
{
    private readonly IUserRepository _repo;

    public GetModeratorsQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ModeratorDto>> Handle(GetModeratorsQuery request, CancellationToken ct)
    {
        List<ModeratorDto> moderators = await _repo.GetModeratorsAsync(ct);

        return moderators;
    }
}
