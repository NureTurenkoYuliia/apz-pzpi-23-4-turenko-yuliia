using Application.Abstractions;
using Application.DTOs.Aquariums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Aquariums.Queries.GetAllByUserId;

public class GetAquariumsByUserIdQueryHandler : IRequestHandler<GetAquariumsByUserIdQuery, List<AquariumDto>>
{
    private readonly IAquariumRepository _repo;
    private readonly ILogger<GetAquariumsByUserIdQueryHandler> _logger;

    public GetAquariumsByUserIdQueryHandler(IAquariumRepository repo, ILogger<GetAquariumsByUserIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<AquariumDto>> Handle(GetAquariumsByUserIdQuery request, CancellationToken ct)
    {
        var userAquariums = await _repo.GetAllByUserIdAsync(request.UserId);

        List<AquariumDto> list = userAquariums.Select(a => new AquariumDto
        {
            Id = a.Id,
            UserId = a.UserId,
            Name = a.Name,
            Location = a.Location,
            IsActive = a.IsActive
        })
        .ToList();

        _logger.LogInformation("USER_ACTION Successfully retrieved aquariums for user: {Id} ", request.UserId);

        return list;
    }
}
