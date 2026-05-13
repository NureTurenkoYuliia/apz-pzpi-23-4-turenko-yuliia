namespace Application.DTOs.Users;

public class ModeratorDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
