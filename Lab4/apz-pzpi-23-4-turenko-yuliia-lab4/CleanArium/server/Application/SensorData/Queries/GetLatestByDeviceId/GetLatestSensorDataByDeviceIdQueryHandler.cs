using Application.Abstractions;
using Application.DTOs.SensorsData;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SensorData.Queries.GetLatestByDeviceId;

public class GetLatestSensorDataByDeviceIdQueryHandler : IRequestHandler<GetLatestSensorDataByDeviceIdQuery, SensorDataDto>
{
    private readonly ISensorDataRepository _repo;
    private readonly ILogger<GetLatestSensorDataByDeviceIdQueryHandler> _logger;

    public GetLatestSensorDataByDeviceIdQueryHandler(ISensorDataRepository repo, ILogger<GetLatestSensorDataByDeviceIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SensorDataDto> Handle(GetLatestSensorDataByDeviceIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetLatestByDeviceAsync(request.DeviceId);

        if (data == null)
        {
            _logger.LogInformation("USER_ACTION No sensor data found for Device: {Id}", request.DeviceId);

            return null;
        }

        SensorDataDto dto = new SensorDataDto
        {
            Id = data.Id,
            DeviceId = data.DeviceId,
            Value = data.Value,
            Unit = data.Unit,
            DateTime = data.DateTime
        };

        _logger.LogInformation("USER_ACTION Successfully retrieved latest sensor data of Device: {Id} ", request.DeviceId);

        return dto;
    }
}