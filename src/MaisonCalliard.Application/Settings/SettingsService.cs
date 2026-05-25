using MaisonCalliard.Application.Settings.Dtos;
using MaisonCalliard.Domain.Repositories;

namespace MaisonCalliard.Application.Settings;

public interface ISettingsService
{
    Task<LeadTimeDto> GetLeadTimeAsync(CancellationToken cancellationToken = default);
    Task<LeadTimeDto> UpdateLeadTimeAsync(UpdateLeadTimeRequest request, CancellationToken cancellationToken = default);
}

internal sealed class SettingsService : ISettingsService
{
    private readonly ISettingsRepository _settingsRepository;

    public SettingsService(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<LeadTimeDto> GetLeadTimeAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _settingsRepository.GetAsync(cancellationToken);
        return new LeadTimeDto { LeadTimeDays = settings.LeadTimeDays };
    }

    public async Task<LeadTimeDto> UpdateLeadTimeAsync(UpdateLeadTimeRequest request, CancellationToken cancellationToken = default)
    {
        var settings = await _settingsRepository.GetAsync(cancellationToken);
        settings.LeadTimeDays = request.LeadTimeDays;
        await _settingsRepository.UpdateAsync(settings, cancellationToken);
        return new LeadTimeDto { LeadTimeDays = settings.LeadTimeDays };
    }
}
