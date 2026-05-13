using Application.Abstractions;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Aquariums.Commands.Create;

public class CreateAquariumCommandHandlerr : IRequestHandler<CreateAquariumCommand, long>
{
    private readonly IAquariumRepository _repo;
    private readonly ILogger<CreateAquariumCommandHandlerr> _logger;

    public CreateAquariumCommandHandlerr(IAquariumRepository repo, ILogger<CreateAquariumCommandHandlerr> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<long> Handle(CreateAquariumCommand request, CancellationToken ct)
    {
        var aquarium = new Aquarium
        {
            UserId = request.UserId,
            Name = request.Name,
            Location = request.Location,
            IsActive = false
        };

        await _repo.AddAsync(aquarium);

        _logger.LogInformation("USER_ACTION Aquarium created: {Id} by User {UserId}", aquarium.Id, request.UserId);

        return aquarium.Id;
    }
}
