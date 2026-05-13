using Domain.Enums;

namespace CleanArium.Contracts.Devices;

public class UpdateDeviceRequest
{
    public long DeviceId { get; set; }
    public DeviceType DeviceType { get; set; }
    public DeviceStatus DeviceStatus { get; set; }
}
