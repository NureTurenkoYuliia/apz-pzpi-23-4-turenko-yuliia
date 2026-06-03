using Domain.Enums;

namespace Domain.Models;

public class AlarmRule : ModelBase
{
    public required long DeviceId { get; set; }
    public required ConditionType Condition { get; set; }
    public float Threshold { get; set; }
    public string Unit { get; set; }
    public bool IsActive { get; set; }

    public virtual Device Device { get; set; }
}
