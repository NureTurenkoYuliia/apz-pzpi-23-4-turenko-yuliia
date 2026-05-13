using Domain.Enums;

namespace Domain.Models;

public class ScheduledCommand : ModelBase
{
    public long DeviceId { get; set; }
    public required CommandType CommandType { get; set; }
    public DateTime StartTime { get; set; }
    public required RepeatMode RepeatMode { get; set; } = RepeatMode.None;
    public int? IntervalMinutes { get; set; }
    public bool IsActive { get; set; }

    public virtual Device Device { get; set; }
}
