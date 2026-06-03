namespace Application.DTOs.Aquariums;

public class AquariumImportDto
{
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public List<DeviceImportDto> Devices { get; set; } = new();
}
