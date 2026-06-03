using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class SensorDataRepository : ISensorDataRepository
{
    private readonly CleanAriumDbContext _db;

    public SensorDataRepository(CleanAriumDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(SensorData data)
    {
        _db.SensorData.Add(data);
        await _db.SaveChangesAsync();
    }

    public async Task<SensorData?> GetLatestByDeviceAsync(long deviceId)
    {
        return await _db.SensorData
            .Where(x => x.DeviceId == deviceId)
            .OrderByDescending(x => x.DateTime)
            .FirstOrDefaultAsync();
    }

    public async Task<List<SensorData>> GetByDeviceAsync(long deviceId, DateTime from, DateTime to)
    {
        return await _db.SensorData
            .Where(x =>
                x.DeviceId == deviceId &&
                x.DateTime >= from &&
                x.DateTime <= to)
            .OrderBy(x => x.DateTime)
            .ToListAsync();
    }

    public async Task<List<SensorData>> GetRecentAsync(long deviceId, TimeSpan interval)
    {
        var from = DateTime.UtcNow.Subtract(interval);

        return await _db.SensorData
            .Where(x =>
                x.DeviceId == deviceId &&
                x.DateTime >= from)
            .OrderBy(x => x.DateTime)
            .ToListAsync();
    }

    public async Task<double?> GetAverageValueAsync(long deviceId, DateTime from, DateTime to)
    {
        return await _db.SensorData
            .Where(x =>
                x.DeviceId == deviceId &&
                x.DateTime >= from &&
                x.DateTime <= to)
            .AverageAsync(x => (double?)x.Value);
    }

    public async Task<double?> GetTrendSlopeAsync(long deviceId, DateTime from, DateTime to)
    {
        var data = await _db.SensorData
            .Where(x =>
                x.DeviceId == deviceId &&
                x.DateTime >= from &&
                x.DateTime <= to)
            .OrderBy(x => x.DateTime)
            .Select(x => new
            {
                x.DateTime,
                x.Value
            })
            .ToListAsync();

        if (data.Count < 2)
            return null;

        var x = data
            .Select(d => d.DateTime.ToOADate())
            .ToArray();

        var y = data
            .Select(d => (double)d.Value)
            .ToArray();

        double xAvg = x.Average();
        double yAvg = y.Average();

        double numerator = 0;
        double denominator = 0;

        for (int i = 0; i < x.Length; i++)
        {
            numerator += (x[i] - xAvg) * (y[i] - yAvg);
            denominator += (x[i] - xAvg) * (x[i] - xAvg);
        }

        if (denominator == 0)
            return null;

        return numerator / denominator;
    }
}
