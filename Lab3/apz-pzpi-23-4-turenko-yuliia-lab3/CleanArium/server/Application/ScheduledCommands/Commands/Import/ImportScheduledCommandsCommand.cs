using Application.DTOs.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.ScheduledCommands.Commands.Import;

public record ImportScheduledCommandsCommand(long UserId, long DeviceId, IFormFile File) : IRequest<ImportResult>;