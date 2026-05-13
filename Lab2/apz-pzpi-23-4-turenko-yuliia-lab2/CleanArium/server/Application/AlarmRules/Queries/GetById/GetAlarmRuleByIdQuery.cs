using Application.DTOs.AlarmRules;
using MediatR;

namespace Application.AlarmRules.Queries.GetById;

public record GetAlarmRuleByIdQuery(long RuleId) : IRequest<AlarmRuleDto>;
