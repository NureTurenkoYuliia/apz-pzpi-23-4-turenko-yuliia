// Component
public interface INotificationService
{
    void Send(string message);
}

// Concrete Components
public class EmailNotificationService : INotificationService
{
    public void Send(string message)
    {
        Console.WriteLine($"Email sent: {message}");
    }
}

public class SmsNotificationService : INotificationService
{
    public void Send(string message)
    {
        Console.WriteLine($"SMS sent: {message}");
    }
}

public class PushNotificationService : INotificationService
{
    public void Send(string message)
    {
        Console.WriteLine($"Push sent: {message}");
    }
}

// Base Decorator
public abstract class NotificationDecorator : INotificationService
{
    protected INotificationService _wrappee;

    protected NotificationDecorator(INotificationService service)
    {
        _wrappee = service;
    }

    public virtual void Send(string message)
    {
        _wrappee.Send(message);
    }
}

// Concrete Decorator: Logging
public class LoggingNotificationDecorator : NotificationDecorator
{
    public LoggingNotificationDecorator(INotificationService service)
        : base(service) { }

    public override void Send(string message)
    {
        Console.WriteLine($"[LOG] Sending message: {message}");
        base.Send(message);
        Console.WriteLine($"[LOG] Message sent successfully");
    }
}

// Concrete Decorator: Retry
public class RetryNotificationDecorator : NotificationDecorator
{
    private readonly int _maxRetries;

    public RetryNotificationDecorator(INotificationService service, int maxRetries = 3)
        : base(service)
    {
        _maxRetries = maxRetries;
    }

    public override void Send(string message)
    {
        int attempt = 0;

        while (true)
        {
            try
            {
                attempt++;
                Console.WriteLine($"[Retry] Attempt {attempt}");

                base.Send(message);

                break;
            }
            catch (Exception)
            {
                if (attempt >= _maxRetries)
                {
                    Console.WriteLine("[Retry] Failed after max attempts");
                    throw;
                }

                Console.WriteLine("[Retry] Retrying...");
                Thread.Sleep(500);
            }
        }
    }
}

// Client
class Program
{
    static void Main()
    {
        INotificationService service = new EmailNotificationService();

        service = new LoggingNotificationDecorator(service);
        service = new RetryNotificationDecorator(service);

        service.Send("message");
    }
}