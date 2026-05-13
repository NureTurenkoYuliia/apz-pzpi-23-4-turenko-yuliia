using Domain.Enums;

namespace CleanArium.Contracts.ScheduledCommands;

public class CreateScheduledCommandRequest
{
    public long DeviceId { get; set; }
    public CommandType CommandType { get; set; }
    public DateTime StartTime { get; set; }
    public RepeatMode RepeatMode { get; set; }
    public int? IntervalMinutes { get; set; }
    public bool IsActive { get; set; }
}
