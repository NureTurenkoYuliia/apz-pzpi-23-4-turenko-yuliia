namespace Application.DTOs.Aquariums;

public class AquariumDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }
}
