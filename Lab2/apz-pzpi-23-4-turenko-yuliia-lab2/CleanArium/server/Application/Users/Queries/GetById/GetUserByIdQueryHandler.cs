using Application.Abstractions;
using Application.DTOs.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _repo;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(IUserRepository repo, ILogger<GetUserByIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId, ct);

        UserDto dto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            IsBlocked = user.IsBlocked,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
        };

        _logger.LogInformation("Successfully retrieved user info: {Id} ", request.UserId);

        return dto;
    }
}