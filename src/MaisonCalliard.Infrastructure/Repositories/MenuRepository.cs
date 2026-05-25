using MaisonCalliard.Domain.Entities;
using MaisonCalliard.Domain.Repositories;
using MaisonCalliard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaisonCalliard.Infrastructure.Repositories;

internal sealed class MenuRepository : IMenuRepository
{
    private readonly AppDbContext _context;

    public MenuRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems.ToListAsync(cancellationToken);
    }

    public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        _context.MenuItems.Update(menuItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
