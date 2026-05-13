using Application.Abstractions;
using MediatR;

namespace Application.AlarmRules.Commands.Deactivate;

public class DeactivateAlarmRuleCommandHandler : IRequestHandler<DeactivateAlarmRuleCommand>
{
    private readonly IAlarmRuleRepository _repo;

    public DeactivateAlarmRuleCommandHandler(IAlarmRuleRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(DeactivateAlarmRuleCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.RuleId, ct);

        entity.IsActive = false;
        await _repo.UpdateAsync(entity, ct);
    }
}
