using Application.Abstractions;
using Application.DTOs.AlarmRules;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.AlarmRules.Queries.GetById;

public class GetAlarmRuleByIdQueryHandler : IRequestHandler<GetAlarmRuleByIdQuery, AlarmRuleDto>
{
    private readonly IAlarmRuleRepository _repo;
    private readonly ILogger<GetAlarmRuleByIdQueryHandler> _logger;

    public GetAlarmRuleByIdQueryHandler(IAlarmRuleRepository repo, ILogger<GetAlarmRuleByIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<AlarmRuleDto> Handle(GetAlarmRuleByIdQuery request, CancellationToken ct)
    {
        var rule = await _repo.GetByIdAsync(request.RuleId, ct);

        AlarmRuleDto dto = new AlarmRuleDto {
            Id = rule.Id,
            DeviceId = rule.DeviceId,
            Condition = rule.Condition,
            Threshold = rule.Threshold,
            Unit = rule.Unit,
            IsActive = rule.IsActive,
        };

        _logger.LogInformation("USER_ACTION Successfully retrieved alarm rule: {Id} ", request.RuleId);

        return dto;
    }
}
