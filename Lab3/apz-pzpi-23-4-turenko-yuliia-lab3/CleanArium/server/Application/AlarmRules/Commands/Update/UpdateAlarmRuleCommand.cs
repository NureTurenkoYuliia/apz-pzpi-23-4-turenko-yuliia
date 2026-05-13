using Domain.Enums;
using MediatR;

namespace Application.AlarmRules.Commands.Update;

public record UpdateAlarmRuleCommand(
    long UserId, 
    long RuleId, 
    ConditionType Condition, 
    float Threshold, 
    string Unit,
    bool IsActive) : IRequest;

