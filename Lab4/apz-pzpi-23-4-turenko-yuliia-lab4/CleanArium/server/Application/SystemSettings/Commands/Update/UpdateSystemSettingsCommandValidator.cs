using FluentValidation;

namespace Application.SystemSettings.Commands.Update;

public class UpdateSystemSettingsCommandValidator : AbstractValidator<UpdateSystemSettingsCommand>
{
    public UpdateSystemSettingsCommandValidator()
    {
        RuleFor(x => x.MaxAquariumsPerUser).GreaterThan(0);
        RuleFor(x => x.MaxDevicesPerAquarium).GreaterThan(0);
        RuleFor(x => x.MaxAlarmRulesPerDevice).GreaterThan(0);
        RuleFor(x => x.MaxScheduledCommandsPerDevice).GreaterThan(0);
    }
}