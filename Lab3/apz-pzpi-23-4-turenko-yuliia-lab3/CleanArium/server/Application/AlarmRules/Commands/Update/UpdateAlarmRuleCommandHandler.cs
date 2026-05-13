using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.AlarmRules.Commands.Update;

public class UpdateAlarmRuleCommandHandler : IRequestHandler<UpdateAlarmRuleCommand>
{
    private readonly IAlarmRuleRepository _repo;
    private readonly ILogger<UpdateAlarmRuleCommandHandler> _logger;

    public UpdateAlarmRuleCommandHandler(IAlarmRuleRepository repo, ILogger<UpdateAlarmRuleCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(UpdateAlarmRuleCommand request, CancellationToken ct)
    {
        var rule = await _repo.GetByIdAsync(request.RuleId, ct);

        rule.Condition = request.Condition;
        rule.Threshold = request.Threshold;
        rule.Unit = request.Unit;
        rule.IsActive = request.IsActive;

        await _repo.UpdateAsync(rule, ct);

        _logger.LogInformation("USER_ACTION Alarm Rule updated: {Id} ", request.RuleId);
    }
}