using FluentValidation;

namespace Application.SensorData.Commands.Create;

public class CreateSensorDataCommandValidator : AbstractValidator<CreateSensorDataCommand>
{
    public CreateSensorDataCommandValidator()
    {
        RuleFor(x => x.DeviceId).GreaterThan(0);
        RuleFor(x => x.Unit).NotEmpty();
    }
}