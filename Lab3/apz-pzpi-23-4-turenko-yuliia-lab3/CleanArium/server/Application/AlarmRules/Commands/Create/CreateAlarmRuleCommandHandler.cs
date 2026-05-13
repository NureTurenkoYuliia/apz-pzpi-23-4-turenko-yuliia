using Application.Abstractions;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.AlarmRules.Commands.Create;

public class CreateAlarmRuleCommandHandler : IRequestHandler<CreateAlarmRuleCommand, long>
{
    private readonly IAlarmRuleRepository _repo;
    private readonly ILogger<CreateAlarmRuleCommandHandler> _logger;

    public CreateAlarmRuleCommandHandler(IAlarmRuleRepository repo, ILogger<CreateAlarmRuleCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<long> Handle(CreateAlarmRuleCommand request, CancellationToken ct)
    {
        var rule = new AlarmRule
        {
            DeviceId = request.DeviceId,
            Condition = request.Condition,
            Threshold = request.Threshold,
            Unit = request.Unit,
            IsActive = true
        };

        await _repo.AddAsync(rule, ct);

        _logger.LogInformation("USER_ACTION Alarm Rule created: {Id} by User {UserId}", rule.Id, request.UserId);

        return rule.Id;
    }
}