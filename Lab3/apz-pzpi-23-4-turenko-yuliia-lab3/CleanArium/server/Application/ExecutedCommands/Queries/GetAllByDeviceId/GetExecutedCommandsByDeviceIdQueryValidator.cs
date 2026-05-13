using Application.Abstractions;
using FluentValidation;

namespace Application.ExecutedCommands.Queries.GetAllByDeviceId;

public class GetExecutedCommandsByDeviceIdQueryValidator : AbstractValidator<GetExecutedCommandsByDeviceIdQuery>
{
    private readonly IDeviceRepository _deviceRepo;

    public GetExecutedCommandsByDeviceIdQueryValidator(IDeviceRepository deviceRepo)
    {
        _deviceRepo = deviceRepo;

        RuleFor(x => x.DeviceId).NotEmpty().GreaterThan(0)
            .WithMessage("Device id is required to get executed commands.");

        RuleFor(x => x)
            .MustAsync(DeviceExists).WithMessage("Device not found.");
    }

    private async Task<bool> DeviceExists(GetExecutedCommandsByDeviceIdQuery cmd, CancellationToken ct)
    {
        return await _deviceRepo.ExistsByIdAsync(cmd.DeviceId, ct);
    }
}