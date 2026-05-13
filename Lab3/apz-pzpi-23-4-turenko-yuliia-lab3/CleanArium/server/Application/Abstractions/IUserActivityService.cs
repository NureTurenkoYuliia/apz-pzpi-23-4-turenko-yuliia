using Application.DTOs.Users;

namespace Application.Abstractions;

public interface IUserActivityService
{
    Task<List<UserActivityDailyDto>> GetDailyStatsAsync(int days);
}
