using MaisonCalliard.Application.Orders.Dtos;
using MaisonCalliard.Domain.Entities;
using MaisonCalliard.Domain.Enums;
using MaisonCalliard.Domain.Repositories;

namespace MaisonCalliard.Application.Orders;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

internal sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : MapToDto(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Items = request.Items.Select(i => new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = i.CartId,
                ProductId = i.ProductId,
                Name = i.Name,
                ImageUrl = i.ImageUrl,
                OptionLabel = i.OptionLabel,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            Total = request.Total,
            TaxAmount = request.TaxAmount,
            PickupDateTime = request.PickupDateTime,
            Location = request.Location,
            CustomerName = request.CustomerName,
            Email = request.Email,
            Phone = request.Phone,
            Message = request.Message,
            Status = OrderStatus.AwaitingPayment,
            CreatedAt = DateTime.UtcNow
        };

        await _orderRepository.AddAsync(order, cancellationToken);
        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {id} not found.");

        if (request.Status == OrderStatus.Pending && order.Status == OrderStatus.AwaitingPayment)
        {
            throw new InvalidOperationException("Order cannot be activated until payment is confirmed.");
        }

        order.Status = request.Status;
        await _orderRepository.UpdateAsync(order, cancellationToken);
        return MapToDto(order);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {id} not found.");

        await _orderRepository.DeleteAsync(order, cancellationToken);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            StripeSessionId = order.StripeSessionId,
            Items = order.Items.Select(i => new CartItemDto
            {
                CartId = i.CartId,
                ProductId = i.ProductId,
                Name = i.Name,
                ImageUrl = i.ImageUrl,
                OptionLabel = i.OptionLabel,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            Total = order.Total,
            TaxAmount = order.TaxAmount,
            PickupDateTime = order.PickupDateTime,
            Location = order.Location,
            CustomerName = order.CustomerName,
            Email = order.Email,
            Phone = order.Phone,
            Message = order.Message,
            Status = order.Status,
            IsPrinted = order.IsPrinted,
            CreatedAt = order.CreatedAt
        };
    }
}
