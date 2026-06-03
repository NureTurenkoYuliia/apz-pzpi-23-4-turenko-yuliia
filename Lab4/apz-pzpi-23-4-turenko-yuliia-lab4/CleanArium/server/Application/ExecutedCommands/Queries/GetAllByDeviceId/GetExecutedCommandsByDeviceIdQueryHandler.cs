using Application.Abstractions;
using Application.DTOs.ExecutedCommands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.ExecutedCommands.Queries.GetAllByDeviceId;

public class GetExecutedCommandsByDeviceIdQueryHandler : IRequestHandler<GetExecutedCommandsByDeviceIdQuery, List<ExecutedCommandDto>>
{
    private readonly IExecutedCommandRepository _repo;
    private readonly ILogger<GetExecutedCommandsByDeviceIdQueryHandler> _logger;

    public GetExecutedCommandsByDeviceIdQueryHandler(IExecutedCommandRepository repo, ILogger<GetExecutedCommandsByDeviceIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<ExecutedCommandDto>> Handle(GetExecutedCommandsByDeviceIdQuery request, CancellationToken cancellationToken)
    {
        var commands = await _repo.GetAllByDeviceIdAsync(request.DeviceId);

        List<ExecutedCommandDto> list = commands.Select(c => new ExecutedCommandDto
        {
            Id = c.Id,
            DeviceId = c.DeviceId,
            CommandType = c.CommandType,
            CommandStatus = c.CommandStatus,
            IssuedAt = c.IssuedAt
        })
        .ToList();

        _logger.LogInformation("USER_ACTION Successfully retrieved executed commands of Device: {Id} ", request.DeviceId);

        return list;
    }
}