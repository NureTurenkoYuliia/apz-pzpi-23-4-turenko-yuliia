using Application.Abstractions;
using Domain.Models;
using Persistence.Application;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public class SensorDataService : ISensorDataService
{
    private readonly CleanAriumDbContext _db;
    private readonly IAlarmRuleProcessor _processor;

    public SensorDataService(CleanAriumDbContext db, IAlarmRuleProcessor processor)
    {
        _db = db;
        _processor = processor;
    }

    public async Task SaveAsync(SensorData data)
    {
        var userId = await _db.SensorData
        .Where(d => d.Id == data.Id)
        .Select(d => d.Device.Aquarium.UserId)
        .FirstOrDefaultAsync();

        _db.SensorData.Add(data);
        await _db.SaveChangesAsync();

        await _processor.ProcessAsync(userId, data);
    }
}