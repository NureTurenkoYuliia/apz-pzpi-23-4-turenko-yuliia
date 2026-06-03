using MediatR;

namespace Application.AlarmRules.Commands.Deactivate;

public record DeactivateAlarmRuleCommand(long RuleId) : IRequest;
