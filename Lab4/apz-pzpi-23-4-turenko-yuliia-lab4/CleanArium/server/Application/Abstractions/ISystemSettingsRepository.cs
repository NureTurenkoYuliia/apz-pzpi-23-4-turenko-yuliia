namespace Application.Abstractions;

public interface ISystemSettingsRepository
{
    Task<Domain.Models.SystemSettings> GetAsync(CancellationToken ct);
    Task UpdateAsync(Domain.Models.SystemSettings settings, CancellationToken ct);
}
