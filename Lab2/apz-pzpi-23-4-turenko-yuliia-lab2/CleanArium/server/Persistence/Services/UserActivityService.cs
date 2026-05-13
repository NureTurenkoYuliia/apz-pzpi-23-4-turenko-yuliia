using Application.Abstractions;
using Application.DTOs.Users;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Persistence.Services;

public class UserActivityService : IUserActivityService
{
    private readonly ILogger<UserActivityService> _logger;

    public UserActivityService(ILogger<UserActivityService> logger)
    {
        _logger = logger;
    }

    public async Task<List<UserActivityDailyDto>> GetDailyStatsAsync(int days)
    {
        var logsDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");

        if (!Directory.Exists(logsDir))
            return new List<UserActivityDailyDto>();

        var latestLog = Directory.GetFiles(logsDir, "app*.log")
            .OrderByDescending(File.GetLastWriteTime)
            .FirstOrDefault();

        if (latestLog == null)
            return new List<UserActivityDailyDto>();

        var result = new Dictionary<DateOnly, int>();

        using var stream = new FileStream(
            latestLog,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite
        );

        using var reader = new StreamReader(stream, Encoding.UTF8);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (!line.Contains("USER_ACTION"))
                continue;

            if (DateTime.TryParse(line[..19], out var date))
            {
                var day = DateOnly.FromDateTime(date);
                result.TryAdd(day, 0);
                result[day]++;
            }
        }

        return result
            .OrderByDescending(x => x.Key)
            .Take(days)
            .Select(x => new UserActivityDailyDto
            {
                Date = x.Key,
                ActionsCount = x.Value
            })
            .ToList();
    }
}
