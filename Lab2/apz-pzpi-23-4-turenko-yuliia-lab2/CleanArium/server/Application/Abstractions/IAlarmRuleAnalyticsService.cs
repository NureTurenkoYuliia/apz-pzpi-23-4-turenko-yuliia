using Application.DTOs.Analitics;
using Domain.Models;

namespace Application.Abstractions;

public interface IAlarmRuleAnalyticsService
{
    Task<AlarmRuleAnalysisDto> AnalyzeAsync(AlarmRule rule, DateTime from, DateTime to);
}
