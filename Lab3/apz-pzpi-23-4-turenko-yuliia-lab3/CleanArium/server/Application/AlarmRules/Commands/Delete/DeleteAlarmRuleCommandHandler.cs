using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Application.AlarmRules.Commands.Delete;

public class DeleteAlarmRuleCommandHandler : IRequestHandler<DeleteAlarmRuleCommand>
{
    private readonly IAlarmRuleRepository _repo;
    private readonly ILogger<DeleteAlarmRuleCommandHandler> _logger;

    public DeleteAlarmRuleCommandHandler(IAlarmRuleRepository repo, ILogger<DeleteAlarmRuleCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(DeleteAlarmRuleCommand request, CancellationToken ct)
    {
        await _repo.DeleteAsync(request.RuleId, ct);

        _logger.LogInformation("USER_ACTION Alarm Rule deleted: {Id} ", request.RuleId);
    }
}