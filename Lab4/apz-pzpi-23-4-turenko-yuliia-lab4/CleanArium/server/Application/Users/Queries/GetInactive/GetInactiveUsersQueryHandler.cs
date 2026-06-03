using Application.Abstractions;
using Application.DTOs.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetInactive;

public class GetInactiveUsersQueryHandler : IRequestHandler<GetInactiveUsersQuery, List<InactiveUserDto>>
{
    private readonly IUserRepository _repo;
    private readonly ILogger<GetInactiveUsersQueryHandler> _logger;

    public GetInactiveUsersQueryHandler(IUserRepository repo, ILogger<GetInactiveUsersQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<InactiveUserDto>> Handle(GetInactiveUsersQuery request, CancellationToken ct)
    {
        if (request.limitDays > 30) throw new ArgumentOutOfRangeException(nameof(request.limitDays));
            
        List<InactiveUserDto> users = await _repo.GetInactiveUsersAsync(request.limitDays, ct);

        _logger.LogInformation("Found {Count} inactive users", users.Count);

        return users;
    }
}