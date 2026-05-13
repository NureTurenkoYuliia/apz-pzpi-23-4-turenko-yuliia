using Application.Abstractions;
using MediatR;

namespace Application.SensorData.Commands.Create;

public class CreateSensorDataCommandHandler : IRequestHandler<CreateSensorDataCommand>
{
    private readonly ISensorDataService _sensorDataService;

    public CreateSensorDataCommandHandler(ISensorDataService sensorDataService)
    {
        _sensorDataService = sensorDataService;
    }

    public async Task Handle(CreateSensorDataCommand command, CancellationToken ct)
    {
        var data = new Domain.Models.SensorData
        {
            DeviceId = command.DeviceId,
            Value = command.Value,
            Unit = command.Unit,
            DateTime = DateTime.UtcNow
        };

        await _sensorDataService.SaveAsync(data);
    }
}