using Domain.Enums;

namespace Application.DTOs.Aquariums;

public class DeviceImportDto
{
    public DeviceType DeviceType { get; set; }
    public DeviceStatus DeviceStatus { get; set; }
}