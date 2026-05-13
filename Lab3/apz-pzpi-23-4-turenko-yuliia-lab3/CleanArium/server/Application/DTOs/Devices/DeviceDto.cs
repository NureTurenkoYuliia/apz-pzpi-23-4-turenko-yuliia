using Domain.Enums;

namespace Application.DTOs.Devices;

public class DeviceDto
{
    public long Id { get; set; }
    public long AquariumId { get; set; }
    public DeviceType DeviceType { get; set; }
    public DeviceStatus DeviceStatus { get; set; }
}
