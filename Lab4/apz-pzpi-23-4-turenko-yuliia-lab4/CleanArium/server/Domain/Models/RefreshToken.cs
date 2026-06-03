namespace Domain.Models;

public class RefreshToken : ModelBase
{
    public long UserId { get; set; }
    public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }

    public virtual User User { get; set; }
}
