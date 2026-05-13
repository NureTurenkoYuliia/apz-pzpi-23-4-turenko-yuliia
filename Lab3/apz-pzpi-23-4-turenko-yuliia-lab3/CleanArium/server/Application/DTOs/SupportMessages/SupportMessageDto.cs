using Domain.Enums;

namespace Application.DTOs.SupportMessages;

public class SupportMessageDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? ReplyToMessageId { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public MessageStatus MessageStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}
