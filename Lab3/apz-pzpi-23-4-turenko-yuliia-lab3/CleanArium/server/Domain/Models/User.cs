using Domain.Enums;

namespace Domain.Models;

public class User : ModelBase
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public required UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<Aquarium> Aquariums { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }
    public virtual ICollection<SupportMessage> SupportMessages { get; set; }
}
