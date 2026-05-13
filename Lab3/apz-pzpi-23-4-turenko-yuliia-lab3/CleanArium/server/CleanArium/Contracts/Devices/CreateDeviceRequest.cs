using Domain.Enums;

namespace CleanArium.Contracts.Devices;

public class CreateDeviceRequest
{
    public long AquariumId { get; set; }
    public DeviceType DeviceType { get; set; }
    public DeviceStatus DeviceStatus { get; set; }
}
