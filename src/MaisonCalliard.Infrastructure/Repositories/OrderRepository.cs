using MaisonCalliard.Domain.Entities;
using MaisonCalliard.Domain.Repositories;
using MaisonCalliard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaisonCalliard.Infrastructure.Repositories;

internal sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders.Include(o => o.Items).ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order?> GetByStripeSessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.StripeSessionId == sessionId, cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
