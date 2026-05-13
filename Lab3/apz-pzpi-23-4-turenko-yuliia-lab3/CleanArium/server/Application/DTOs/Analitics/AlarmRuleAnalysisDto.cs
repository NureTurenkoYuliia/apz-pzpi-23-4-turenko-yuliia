namespace Application.DTOs.Analitics;

public class AlarmRuleAnalysisDto
{
    public long AlarmRuleId { get; set; }
    public double AverageValue { get; set; }
    public double TrendPerDay { get; set; }
    public int EstimatedDaysToTrigger { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}
