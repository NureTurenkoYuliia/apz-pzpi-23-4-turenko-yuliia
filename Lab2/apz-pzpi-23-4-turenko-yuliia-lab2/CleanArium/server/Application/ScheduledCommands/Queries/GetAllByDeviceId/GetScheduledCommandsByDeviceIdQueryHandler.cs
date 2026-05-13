using Application.Abstractions;
using Application.DTOs.ScheduledCommands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.ScheduledCommands.Queries.GetAllByDeviceId;

public class GetScheduledCommandsByDeviceIdQueryHandler : IRequestHandler<GetScheduledCommandsByDeviceIdQuery, List<ScheduledCommandDto>>
{
    private readonly IScheduledCommandRepository _repo;
    private readonly ILogger<GetScheduledCommandsByDeviceIdQueryHandler> _logger;

    public GetScheduledCommandsByDeviceIdQueryHandler(IScheduledCommandRepository repo, ILogger<GetScheduledCommandsByDeviceIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<ScheduledCommandDto>> Handle(GetScheduledCommandsByDeviceIdQuery request, CancellationToken cancellationToken)
    {
        var commands = await _repo.GetAllByDeviceIdAsync(request.DeviceId);

        List<ScheduledCommandDto> list = commands.Select(c => new ScheduledCommandDto
        {
            Id = c.Id,
            DeviceId = c.DeviceId,
            CommandType = c.CommandType,
            StartTime = c.StartTime,
            RepeatMode = c.RepeatMode,
            IntervalMinutes = c.IntervalMinutes,
            IsActive = c.IsActive
        })
        .ToList();

        _logger.LogInformation("USER_ACTION Successfully retrieved scheduled commands for User: {Id} ", request.UserId);

        return list;
    }
}