namespace Application.DTOs.Users;

public class UserDto
{
    public long Id { get; set; }
    public string Email { get; set; } = default!;
    public bool IsBlocked { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
