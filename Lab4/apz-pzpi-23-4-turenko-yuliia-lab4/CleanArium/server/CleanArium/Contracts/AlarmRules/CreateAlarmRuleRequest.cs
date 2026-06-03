using Domain.Enums;

namespace CleanArium.Contracts.AlarmRules;

public class CreateAlarmRuleRequest
{
    public long DeviceId { get; set; }
    public ConditionType Condition { get; set; }
    public float Threshold { get; set; }
    public string Unit { get; set; }
}
