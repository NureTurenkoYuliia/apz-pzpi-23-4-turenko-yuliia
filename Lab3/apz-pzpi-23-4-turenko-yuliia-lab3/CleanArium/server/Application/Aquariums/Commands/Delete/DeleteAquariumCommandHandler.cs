using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Aquariums.Commands.Delete;

public class DeleteAquariumCommandHandler : IRequestHandler<DeleteAquariumCommand>
{
    private readonly IAquariumRepository _repo;
    private readonly ILogger<DeleteAquariumCommandHandler> _logger;

    public DeleteAquariumCommandHandler(IAquariumRepository repo, ILogger<DeleteAquariumCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(DeleteAquariumCommand request, CancellationToken ct)
    {
        var aquarium = await _repo.GetByIdAsync(request.AquariumId);

        await _repo.DeleteAsync(aquarium);

        _logger.LogWarning("USER_ACTION Aquarium {Id} deleted", request.AquariumId);
    }
}
