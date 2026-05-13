using Application.DTOs.Analitics;

namespace Application.Abstractions;

public interface ICommandAlarmAnalyticsService
{
    Task<List<CommandAlarmCorrelationDto>> AnalyzeAsync(DateTime from, DateTime to, TimeSpan maxDelay);
}