namespace Domain.Models;

public class PasswordResetToken : ModelBase
{
    public long UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;

    public virtual User User { get; set; }
}
