using Application.Abstractions;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.ScheduledCommands.Commands.Create;

public class CreateScheduledCommandHandler : IRequestHandler<CreateScheduledCommand, long>
{
    private readonly IScheduledCommandRepository _repo;
    private readonly ILogger<CreateScheduledCommandHandler> _logger;

    public CreateScheduledCommandHandler(IScheduledCommandRepository repo, ILogger<CreateScheduledCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<long> Handle(CreateScheduledCommand request, CancellationToken cancellationToken)
    {
        var entity = new ScheduledCommand
        {
            DeviceId = request.DeviceId,
            CommandType = request.CommandType,
            StartTime = request.StartTime,
            RepeatMode = request.RepeatMode,
            IntervalMinutes = request.IntervalMinutes,
            IsActive = request.IsActive
        };

        await _repo.AddAsync(entity);

        _logger.LogInformation("USER_ACTION ScheduledCommand created: {Id}", entity.Id);

        return entity.Id;
    }
}