using Domain.Enums;

namespace CleanArium.Contracts.Devices
{
    public class CreateExecutedCommandRequest
    {
        public required CommandType CommandType { get; set; }
        public required CommandStatus CommandStatus { get; set; }
    }
}
