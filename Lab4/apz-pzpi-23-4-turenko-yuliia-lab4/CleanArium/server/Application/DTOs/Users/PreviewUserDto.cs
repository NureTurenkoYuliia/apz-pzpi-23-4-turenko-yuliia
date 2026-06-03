namespace Application.DTOs.Users;

public class PreviewUserDto
{
    public long UserId { get; set; }
    public string Email { get; set; } = default!;
    public DateTime? LastLoginAt { get; set; }
}
