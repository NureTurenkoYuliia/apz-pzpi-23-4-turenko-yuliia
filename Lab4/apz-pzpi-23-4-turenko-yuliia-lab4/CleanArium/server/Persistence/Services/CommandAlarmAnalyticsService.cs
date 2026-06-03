using Application.Abstractions;
using Application.DTOs.Analitics;

namespace Persistence.Services;

public class CommandAlarmAnalyticsService : ICommandAlarmAnalyticsService
{
    private readonly IExecutedCommandRepository _commandsRepo;
    private readonly INotificationRepository _notificationsRepo;

    public CommandAlarmAnalyticsService(
        IExecutedCommandRepository commandsRepo,
        INotificationRepository notificationsRepo)
    {
        _commandsRepo = commandsRepo;
        _notificationsRepo = notificationsRepo;
    }

    public async Task<List<CommandAlarmCorrelationDto>> AnalyzeAsync(DateTime from, DateTime to, TimeSpan maxDelay)
    {
        var commands = await _commandsRepo.GetByPeriodAsync(from, to);

        var alarms = await _notificationsRepo.GetAlarmNotificationsByPeriodAsync(from, to);

        var result = new List<CommandAlarmCorrelationDto>();

        foreach (var deviceGroup in commands.GroupBy(c => c.DeviceId))
        {
            foreach (var cmdGroup in deviceGroup.GroupBy(c => c.CommandType))
            {
                var relatedAlarms = alarms
                    .Where(a => a.AlarmRuleId != null)
                    .ToList();

                var delays = new List<TimeSpan>();

                foreach (var cmd in cmdGroup)
                {
                    var alarm = relatedAlarms
                        .Where(a => a.CreatedAt >= cmd.IssuedAt && a.CreatedAt <= cmd.IssuedAt + maxDelay)
                        .OrderBy(a => a.CreatedAt)
                        .FirstOrDefault();

                    if (alarm != null)
                        delays.Add(alarm.CreatedAt - cmd.IssuedAt);
                }

                if (!delays.Any())
                    continue;

                result.Add(new CommandAlarmCorrelationDto
                {
                    DeviceId = deviceGroup.Key,
                    CommandType = cmdGroup.Key,
                    CommandCount = cmdGroup.Count(),
                    AlarmCount = delays.Count,
                    AvgDelayBetweenCommandAndAlarm =
                        TimeSpan.FromSeconds(delays.Average(d => d.TotalSeconds)),
                    Recommendation = BuildRecommendation(cmdGroup.Count(), delays.Count)
                });
            }
        }

        return result;
    }

    private static string BuildRecommendation(int commandCount, int alarmCount)
    {
        var ratio = (double)alarmCount / commandCount;

        if (ratio > 0.7)
            return "High correlation: consider disabling command or adjusting alarm rule thresholds";

        if (ratio > 0.4)
            return "Moderate correlation: review command timing or alarm rule sensitivity";

        return "Low correlation: no action required";
    }
}
