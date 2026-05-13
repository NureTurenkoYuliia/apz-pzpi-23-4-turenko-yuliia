namespace Application.Abstractions;

public interface IAlarmRuleProcessor
{
    Task ProcessAsync(long userId, Domain.Models.SensorData data);
}
