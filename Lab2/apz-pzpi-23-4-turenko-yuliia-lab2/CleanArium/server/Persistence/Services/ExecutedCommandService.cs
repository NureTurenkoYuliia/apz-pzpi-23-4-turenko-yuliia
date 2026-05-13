using Application.Abstractions;
using Domain.Enums;
using Domain.Models;

namespace Persistence.Services;

public class ExecutedCommandService : IExecutedCommandService
{
    private readonly IExecutedCommandRepository _repo;

    public ExecutedCommandService(IExecutedCommandRepository repo)
    {
        _repo = repo;
    }

    public async Task RegisterAsync(long deviceId, CommandType commandType, CommandStatus status)
    {
        var entity = new ExecutedCommand
        {
            DeviceId = deviceId,
            CommandType = commandType,
            CommandStatus = status,
            IssuedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
    }
}
