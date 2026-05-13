using Domain.Enums;

namespace CleanArium.Contracts.AlarmRules;

public class UpdateAlarmRuleRequest
{
    public long RuleId { get; set; }
    public ConditionType Condition { get; set; }
    public float Threshold { get; set; }
    public string Unit { get; set; }
    public bool IsActive { get; set; }
}
