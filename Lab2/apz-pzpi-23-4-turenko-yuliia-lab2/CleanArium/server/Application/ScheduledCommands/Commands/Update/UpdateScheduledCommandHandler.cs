using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.ScheduledCommands.Commands.Update;

public class UpdateScheduledCommandHandler : IRequestHandler<UpdateScheduledCommand>
{
    private readonly IScheduledCommandRepository _repo;
    private readonly ILogger<UpdateScheduledCommandHandler> _logger;

    public UpdateScheduledCommandHandler(IScheduledCommandRepository repo, ILogger<UpdateScheduledCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(UpdateScheduledCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.CommandId, ct);

        entity.CommandType = request.CommandType;
        entity.StartTime = request.StartTime;
        entity.RepeatMode = request.RepeatMode;
        entity.IntervalMinutes = request.IntervalMinutes;
        entity.IsActive = request.IsActive;

        await _repo.UpdateAsync(entity);

        _logger.LogInformation("USER_ACTION ScheduledCommand updated: {Id}", request.CommandId);
    }
}