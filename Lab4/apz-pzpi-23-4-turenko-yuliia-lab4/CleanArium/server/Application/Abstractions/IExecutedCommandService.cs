using Domain.Enums;

namespace Application.Abstractions;

public interface IExecutedCommandService
{
    Task RegisterAsync(long deviceId, CommandType commandType, CommandStatus status);
}

