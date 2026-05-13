using Application.Abstractions;
using Application.DTOs.AlarmRules;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Application.AlarmRules.Queries.GetAllByDeviceId;

public class GetAlarmRulesByDeviceIdQueryHandler : IRequestHandler<GetAlarmRulesByDeviceIdQuery, List<AlarmRuleDto>>
{
    private readonly IAlarmRuleRepository _repo;
    private readonly ILogger<GetAlarmRulesByDeviceIdQueryHandler> _logger;

    public GetAlarmRulesByDeviceIdQueryHandler(IAlarmRuleRepository repo, ILogger<GetAlarmRulesByDeviceIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<AlarmRuleDto>> Handle(GetAlarmRulesByDeviceIdQuery request, CancellationToken ct)
    {
        var rules = await _repo.GetAllByDeviceIdAsync(request.DeviceId, ct);

        List<AlarmRuleDto> list = rules.Select(r => new AlarmRuleDto
        {
            Id = r.Id,
            DeviceId = r.DeviceId,
            Condition = r.Condition,
            Threshold = r.Threshold,
            Unit = r.Unit,
            IsActive = r.IsActive,
        })
        .ToList();

        _logger.LogInformation("USER_ACTION Successfully retrieved alarm rules for device: {Id} ", request.DeviceId);

        return list;
    }
}