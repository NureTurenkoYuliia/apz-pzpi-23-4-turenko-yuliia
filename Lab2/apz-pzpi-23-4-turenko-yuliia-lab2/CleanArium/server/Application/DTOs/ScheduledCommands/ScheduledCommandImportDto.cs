using Domain.Enums;

namespace Application.DTOs.ScheduledCommands;

public class ScheduledCommandImportDto
{
    public CommandType CommandType { get; set; }
    public DateTime StartTime { get; set; }
    public RepeatMode RepeatMode { get; set; }
    public int? IntervalMinutes { get; set; }
    public bool IsActive { get; set; }
}
