using Application.DTOs.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.AlarmRules.Commands.Import;

public record ImportAlarmRulesCommand(long UserId, long DeviceId, IFormFile File) : IRequest<ImportResult>;
