using MediatR;

namespace Application.AlarmRules.Commands.Delete;

public record DeleteAlarmRuleCommand(long UserId, long RuleId) : IRequest;
