using Application.Abstractions;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Persistence.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository repo, ILogger<NotificationService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task CreateForAlarmRulesAsync(long userId, string title, string content, AlarmRule rule)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            AlarmRule = rule,
            AlarmRuleId = rule.Id
        };

        await _repo.AddAsync(notification);

        _logger.LogInformation("Notification '{Title}' sent to User: {UserId}", title, userId);
    }

    public async Task CreateForCommandsAsync(long userId, string title, string content, ScheduledCommand command)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            ScheduledCommand = command,
            ScheduledCommandId = command.Id
        };

        await _repo.AddAsync(notification);

        _logger.LogInformation("Notification '{Title}' sent to User: {UserId}", title, userId);
    }
}