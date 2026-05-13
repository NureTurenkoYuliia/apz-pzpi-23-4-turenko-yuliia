using Domain.Models;

namespace Application.Abstractions;

public interface INotificationService
{
    Task CreateForAlarmRulesAsync(long userId, string title, string content, AlarmRule rule);
    Task CreateForCommandsAsync(long userId, string title, string content, ScheduledCommand command);
}
