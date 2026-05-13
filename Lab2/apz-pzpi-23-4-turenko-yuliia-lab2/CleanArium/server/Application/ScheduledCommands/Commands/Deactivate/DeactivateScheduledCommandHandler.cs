using Application.Abstractions;
using MediatR;

namespace Application.ScheduledCommands.Commands.Deactivate;

public class DeactivateScheduledCommandHandler : IRequestHandler<DeactivateScheduledCommand>
{
    private readonly IScheduledCommandRepository _repo;

    public DeactivateScheduledCommandHandler(IScheduledCommandRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(DeactivateScheduledCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.CommandId, ct);

        entity.IsActive = false;

        await _repo.UpdateAsync(entity);
    }
}
