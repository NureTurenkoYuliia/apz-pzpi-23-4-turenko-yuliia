namespace Application.DTOs.SensorsData;

public class SensorDataDto
{
    public long Id { get; set; }
    public long DeviceId { get; set; }
    public float Value { get; set; }
    public string Unit { get; set; }
    public DateTime DateTime { get; set; }
}
