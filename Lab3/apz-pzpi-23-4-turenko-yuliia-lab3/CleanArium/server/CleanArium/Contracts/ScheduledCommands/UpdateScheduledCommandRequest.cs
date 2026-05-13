using Domain.Enums;

namespace CleanArium.Contracts.ScheduledCommands;

public class UpdateScheduledCommandRequest
{
    public long Id { get; set; }
    public CommandType CommandType { get; set; }
    public DateTime StartTime { get; set; }
    public RepeatMode RepeatMode { get; set; }
    public int? IntervalMinutes { get; set; }
    public bool IsActive { get; set; }
}
