namespace CleanArium.Contracts.SupportMessages;

public class ReplyMessageRequest
{
    public long MessageId { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
}
