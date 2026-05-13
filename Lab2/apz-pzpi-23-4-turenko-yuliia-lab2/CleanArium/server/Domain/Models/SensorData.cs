namespace Domain.Models;

public class SensorData : ModelBase
{
    public long DeviceId { get; set; }
    public float Value { get; set; }
    public string Unit { get; set; }
    public DateTime DateTime { get; set; }

    public virtual Device Device { get; set; }
}
