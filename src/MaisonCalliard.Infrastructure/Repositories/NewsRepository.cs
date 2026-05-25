using MaisonCalliard.Domain.Entities;
using MaisonCalliard.Domain.Repositories;
using MaisonCalliard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaisonCalliard.Infrastructure.Repositories;

internal sealed class NewsRepository : INewsRepository
{
    private readonly AppDbContext _context;

    public NewsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<NewsItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.NewsItems.ToListAsync(cancellationToken);
    }

    public async Task<NewsItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.NewsItems.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(NewsItem newsItem, CancellationToken cancellationToken = default)
    {
        _context.NewsItems.Add(newsItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(NewsItem newsItem, CancellationToken cancellationToken = default)
    {
        _context.NewsItems.Update(newsItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(NewsItem newsItem, CancellationToken cancellationToken = default)
    {
        _context.NewsItems.Remove(newsItem);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
