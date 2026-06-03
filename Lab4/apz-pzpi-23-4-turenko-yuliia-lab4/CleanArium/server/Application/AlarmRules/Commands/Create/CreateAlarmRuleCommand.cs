using Domain.Enums;
using MediatR;


namespace Application.AlarmRules.Commands.Create;

public record CreateAlarmRuleCommand(
    long UserId, 
    long DeviceId, 
    ConditionType Condition, 
    float Threshold, 
    string Unit) : IRequest<long>;