using Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Persistence.Services.Token;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefreshTokenCleanupService> _logger;

    public RefreshTokenCleanupService(IServiceScopeFactory scopeFactory, ILogger<RefreshTokenCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Refresh token cleanup service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

                var deletedCount = await repo.DeleteExpiredAsync();

                if (deletedCount > 0)
                {
                    _logger.LogInformation("Deleted {Count} expired refresh tokens.", deletedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while cleaning up refresh tokens.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}