using Domain.Enums;

namespace Domain.Models;

public class ExecutedCommand : ModelBase
{
    public long DeviceId { get; set; }
    public required CommandType CommandType { get; set; }
    public required CommandStatus CommandStatus { get; set; }
    public DateTime IssuedAt { get; set; }

    public virtual Device Device { get; set; }
}
