using Domain.Enums;

namespace Application.DTOs.AlarmRules;

public class AlarmRuleImportDto
{
    public ConditionType Condition { get; set; }
    public float Threshold { get; set; }
    public string Unit { get; set; }
}
