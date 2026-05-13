namespace CleanArium.Contracts.SupportMessages;

public class UpdateMessageRequest
{
    public long Id { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
}
