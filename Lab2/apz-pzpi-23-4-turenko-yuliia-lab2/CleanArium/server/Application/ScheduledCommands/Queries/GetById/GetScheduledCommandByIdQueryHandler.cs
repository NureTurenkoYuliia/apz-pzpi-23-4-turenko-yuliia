using Application.Abstractions;
using Application.DTOs.ScheduledCommands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.ScheduledCommands.Queries.GetById;

public class GetScheduledCommandByIdQueryHandler : IRequestHandler<GetScheduledCommandByIdQuery, ScheduledCommandDto>
{
    private readonly IScheduledCommandRepository _repo;
    private readonly ILogger<GetScheduledCommandByIdQueryHandler> _logger;

    public GetScheduledCommandByIdQueryHandler(IScheduledCommandRepository repo, ILogger<GetScheduledCommandByIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<ScheduledCommandDto> Handle(GetScheduledCommandByIdQuery request, CancellationToken ct)
    {
        var command = await _repo.GetByIdAsync(request.CommandId, ct);

        ScheduledCommandDto dto = new ScheduledCommandDto
        {
            Id = command.Id,
            DeviceId = command.DeviceId,
            CommandType = command.CommandType,
            StartTime = command.StartTime,
            RepeatMode = command.RepeatMode,
            IntervalMinutes = command.IntervalMinutes,
            IsActive = command.IsActive
        };

        _logger.LogInformation("USER_ACTION Successfully retrieved scheduled command: {Id} ", request.CommandId);

        return dto;
    }
}