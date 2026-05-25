using MaisonCalliard.Domain.Entities;

namespace MaisonCalliard.Domain.Repositories;

public interface ISettingsRepository
{
    Task<AppSettings> GetAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(AppSettings settings, CancellationToken cancellationToken = default);
}
