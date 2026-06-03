using Domain.Enums;

namespace Application.DTOs.ExecutedCommands;

public class ExecutedCommandDto
{
    public long Id { get; set; }
    public long DeviceId { get; set; }
    public required CommandType CommandType { get; set; }
    public required CommandStatus CommandStatus { get; set; }
    public DateTime IssuedAt { get; set; }
}
