namespace Application.DTOs.Users;

public class InactiveUserDto
{
    public long UserId { get; set; }
    public string Email { get; set; } = default!;
    public DateTime? LastLoginAt { get; set; }
    public int AquariumsCount { get; set; }
    public int ActiveAquariums { get; set; }
    public int ActiveDevices { get; set; }
}
