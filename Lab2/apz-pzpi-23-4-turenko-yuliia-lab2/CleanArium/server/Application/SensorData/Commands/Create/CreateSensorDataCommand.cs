using MediatR;

namespace Application.SensorData.Commands.Create;

public record CreateSensorDataCommand(
    long DeviceId,
    float Value,
    string Unit) : IRequest;
