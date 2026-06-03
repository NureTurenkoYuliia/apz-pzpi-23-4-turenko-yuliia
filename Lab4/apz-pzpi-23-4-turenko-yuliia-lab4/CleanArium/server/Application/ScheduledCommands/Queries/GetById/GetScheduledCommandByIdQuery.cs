using Application.DTOs.ScheduledCommands;
using MediatR;

namespace Application.ScheduledCommands.Queries.GetById;

public record GetScheduledCommandByIdQuery(long CommandId) : IRequest<ScheduledCommandDto>;