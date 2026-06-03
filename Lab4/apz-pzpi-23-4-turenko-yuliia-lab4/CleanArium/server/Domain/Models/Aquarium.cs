namespace Domain.Models;

public class Aquarium : ModelBase
{
    public long UserId { get; set; }
    public required string Name { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; }
    public virtual ICollection<Device> Devices { get; set; }
}
