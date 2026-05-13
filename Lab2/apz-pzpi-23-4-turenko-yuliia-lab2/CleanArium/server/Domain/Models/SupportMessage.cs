using Domain.Enums;

namespace Domain.Models;

public class SupportMessage : ModelBase
{
    public long UserId { get; set; }
    public long? ReplyToMessageId { get; set; }
    public UserRole Sender { get; set; }
    public required string Subject { get; set; }
    public required string Message { get; set; }
    public MessageStatus MessageStatus { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; }
    public virtual SupportMessage? ReplyToMessage { get; set; }
}
