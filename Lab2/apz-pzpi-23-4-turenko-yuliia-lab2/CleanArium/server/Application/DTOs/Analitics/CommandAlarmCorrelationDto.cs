using Domain.Enums;

namespace Application.DTOs.Analitics;

public class CommandAlarmCorrelationDto
{
    public long DeviceId { get; set; }
    public CommandType CommandType { get; set; }
    public int CommandCount { get; set; }
    public int AlarmCount { get; set; }
    public TimeSpan AvgDelayBetweenCommandAndAlarm { get; set; }
    public string Recommendation { get; set; }
}
