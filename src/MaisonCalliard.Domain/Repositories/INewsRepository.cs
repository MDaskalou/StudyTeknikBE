using MaisonCalliard.Domain.Entities;

namespace MaisonCalliard.Domain.Repositories;

public interface INewsRepository
{
    Task<IReadOnlyList<NewsItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<NewsItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(NewsItem newsItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(NewsItem newsItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(NewsItem newsItem, CancellationToken cancellationToken = default);
}
