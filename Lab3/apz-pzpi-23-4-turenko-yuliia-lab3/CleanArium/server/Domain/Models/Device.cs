using Domain.Enums;

namespace Domain.Models;

public class Device : ModelBase
{
    public long AquariumId { get; set; }
    public required DeviceType DeviceType { get; set; }
    public required DeviceStatus DeviceStatus { get; set; }

    public virtual Aquarium Aquarium { get; set; }
    public virtual ICollection<SensorData> SensorData { get; set; }
    public virtual ICollection<ExecutedCommand> ExecutedCommands { get; set; }
    public virtual ICollection<ScheduledCommand> ScheduledCommands { get; set; }
    public virtual ICollection<AlarmRule> AlarmRules { get; set; }
}
