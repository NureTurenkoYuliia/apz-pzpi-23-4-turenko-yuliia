using Application.DTOs.SensorsData;
using MediatR;

namespace Application.SensorData.Queries.GetLatestByDeviceId;

public record GetLatestSensorDataByDeviceIdQuery(long DeviceId) : IRequest<SensorDataDto>;