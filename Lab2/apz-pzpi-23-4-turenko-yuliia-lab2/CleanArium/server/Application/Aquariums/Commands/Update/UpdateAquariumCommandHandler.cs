using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Aquariums.Commands.Update;

public class UpdateAquariumCommandHandler : IRequestHandler<UpdateAquariumCommand>
{
    private readonly IAquariumRepository _repo;
    private readonly ILogger<UpdateAquariumCommandHandler> _logger;

    public UpdateAquariumCommandHandler(IAquariumRepository repo, ILogger<UpdateAquariumCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(UpdateAquariumCommand request, CancellationToken ct)
    {
        var aquarium = await _repo.GetByIdAsync(request.AquariumId);

        aquarium.Name = request.Name;
        aquarium.Location = request.Location;

        await _repo.UpdateAsync(aquarium);

        _logger.LogInformation("USER_ACTION Aquarium {Id} updated", request.AquariumId);
    }
}
