using Application.Abstractions;
using Application.DTOs.Analitics;
using Domain.Models;

namespace Persistence.Services;

public class AlarmRuleAnalyticsService : IAlarmRuleAnalyticsService
{
    private readonly ISensorDataRepository _sensorRepo;

    public AlarmRuleAnalyticsService(ISensorDataRepository sensorRepo)
    {
        _sensorRepo = sensorRepo;
    }

    public async Task<AlarmRuleAnalysisDto> AnalyzeAsync(AlarmRule rule, DateTime from, DateTime to)
    {
        var data = await _sensorRepo.GetByDeviceAsync(rule.DeviceId, from, to);

        if (data.Count < 2)
            return Empty(rule.Id);

        var ordered = data.OrderBy(x => x.DateTime).ToList();

        var avg = ordered.Average(x => x.Value);

        var first = ordered.First();
        var last = ordered.Last();

        var days = Math.Max(
            (last.DateTime - first.DateTime).TotalDays,
            1);

        var trendPerDay = (last.Value - first.Value) / days;

        int estimate = int.MaxValue;
        if (Math.Abs(trendPerDay) > 0.0001)
        {
            estimate = (int)((rule.Threshold - last.Value) / trendPerDay);
            if (estimate < 0) estimate = 0;
        }

        return new AlarmRuleAnalysisDto
        {
            AlarmRuleId = rule.Id,
            AverageValue = Math.Round(avg, 2),
            TrendPerDay = Math.Round(trendPerDay, 3),
            EstimatedDaysToTrigger = estimate,
            Recommendation = BuildRecommendation(rule, trendPerDay, estimate)
        };
    }

    private static AlarmRuleAnalysisDto Empty(long ruleId) => 
        new()
        {
             AlarmRuleId = ruleId,
             Recommendation = "Insufficient data for analysis"
        };

    private static string BuildRecommendation(AlarmRule rule, double trend, int days)
    {
        if (days <= 1)
            return "High risk: Alarm likely to trigger soon. Consider adjusting threshold or device behavior.";

        if (trend > 0)
            return "Value is increasing. Monitor device closely.";

        if (trend < 0)
            return "Value is decreasing. Alarm risk is lowering.";

        return "Stable values. No action needed.";
    }
}
