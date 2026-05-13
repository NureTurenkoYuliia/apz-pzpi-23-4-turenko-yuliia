using Application.Abstractions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Application;

namespace Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly CleanAriumDbContext _db;

    public NotificationRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Notification notification)
    {
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(long userId)
    {
        return await _db.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> CountUnreadAsync(long userId)
    {
        return await _db.Notifications
            .CountAsync(x => x.UserId == userId && !x.IsRead);
    }

    public async Task MarkAsReadAsync(long notificationId)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId);
        if (n == null) return;

        n.IsRead = true;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(long userId, long notificationId, CancellationToken ct)
        => await _db.Notifications.AnyAsync(x => x.UserId == userId && x.Id == notificationId, ct);

    public async Task<List<Notification>> GetAlarmNotificationsByPeriodAsync(DateTime from, DateTime to)
    {
        return await _db.Notifications
            .Where(n =>n.AlarmRuleId != null && n.CreatedAt >= from && n.CreatedAt <= to)
            .AsNoTracking()
            .ToListAsync();
    }
}
