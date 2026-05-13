using Application.Abstractions;
using Domain.Enums;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public class AlarmRuleProcessor : IAlarmRuleProcessor
{
    private readonly CleanAriumDbContext _db;
    private readonly INotificationService _notificationsService;

    public AlarmRuleProcessor(
        CleanAriumDbContext db,
        INotificationService notificationsService)
    {
        _db = db;
        _notificationsService = notificationsService;
    }

    public async Task ProcessAsync(long userId, SensorData data)
    {
        var rules = await _db.AlarmRules
            .Where(r => r.DeviceId == data.DeviceId && r.IsActive)
            .ToListAsync();

        foreach (var rule in rules)
        {
            if (IsTriggered(rule, data.Value))
            {
                await _notificationsService.CreateForAlarmRulesAsync(
                    userId,
                    $"Alarm triggered for device: {rule.Device.DeviceType}",
                    $"Sensor value {data.Value} violated rule {rule.Condition} {rule.Threshold} {rule.Unit}",
                    rule);
            }
        }
    }

    private static bool IsTriggered(AlarmRule rule, float value)
        => rule.Condition switch
        {
            ConditionType.Greater => value > rule.Threshold,
            ConditionType.Less => value < rule.Threshold,
            ConditionType.LessOrEqual => value <= rule.Threshold,
            ConditionType.GreaterOrEqual => value >= rule.Threshold,
            ConditionType.Equal => value == rule.Threshold,
            ConditionType.NotEqual => value != rule.Threshold,
            _ => false
        };
}
