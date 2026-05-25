using MaisonCalliard.Domain.Entities;
using MaisonCalliard.Domain.Repositories;
using MaisonCalliard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaisonCalliard.Infrastructure.Repositories;

internal sealed class SettingsRepository : ISettingsRepository
{
    private readonly AppDbContext _context;

    public SettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AppSettings> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Settings.FirstAsync(cancellationToken);
    }

    public async Task UpdateAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        _context.Settings.Update(settings);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
