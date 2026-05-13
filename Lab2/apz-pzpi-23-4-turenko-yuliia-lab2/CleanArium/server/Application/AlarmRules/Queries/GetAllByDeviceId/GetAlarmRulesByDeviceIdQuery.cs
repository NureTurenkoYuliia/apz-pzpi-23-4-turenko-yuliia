using Application.DTOs.AlarmRules;
using MediatR;

namespace Application.AlarmRules.Queries.GetAllByDeviceId;

public record GetAlarmRulesByDeviceIdQuery(long UserId, long DeviceId) : IRequest<List<AlarmRuleDto>>;