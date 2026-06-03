using Domain.Enums;
using MediatR;

namespace Application.ExecutedCommands.Commands.Create;

public record CreateExecutedCommand(
    long DeviceId,
    CommandType CommandType,
    CommandStatus CommandStatus) : IRequest;
