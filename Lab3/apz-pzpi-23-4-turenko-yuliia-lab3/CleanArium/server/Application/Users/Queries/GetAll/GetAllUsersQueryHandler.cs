using Application.Abstractions;
using Application.DTOs.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetAll;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<PreviewUserDto>>
{
    private readonly IUserRepository _repo;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(IUserRepository repo, ILogger<GetAllUsersQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<PreviewUserDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        List<PreviewUserDto> users = await _repo.GetUsersAsync(ct);

        _logger.LogInformation("Retrieved all users: {Count}", users.Count);

        return users;
    }
}