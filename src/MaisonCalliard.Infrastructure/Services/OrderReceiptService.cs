using MaisonCalliard.Application.Receipts;
using MaisonCalliard.Domain.Entities;
using MaisonCalliard.Domain.Enums;
using MaisonCalliard.Domain.Repositories;
using MaisonCalliard.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaisonCalliard.Infrastructure.Services;

internal sealed class OrderReceiptService : IOrderReceiptService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ResendOrderReceiptSender _sender;
    private readonly ReceiptOptions _receiptOptions;
    private readonly ILogger<OrderReceiptService> _logger;

    public OrderReceiptService(
        IOrderRepository orderRepository,
        ResendOrderReceiptSender sender,
        IOptions<ReceiptOptions> receiptOptions,
        ILogger<OrderReceiptService> logger)
    {
        _orderRepository = orderRepository;
        _sender = sender;
        _receiptOptions = receiptOptions.Value;
        _logger = logger;
    }

    public async Task TrySendReceiptAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found for receipt.", orderId);
            return;
        }

        if (order.ReceiptSentAt is not null)
        {
            return;
        }

        if (order.Status is not (OrderStatus.Pending or OrderStatus.Completed or OrderStatus.Paid))
        {
            _logger.LogDebug(
                "Order {OrderId} status {Status} not eligible for receipt.",
                orderId,
                order.Status);
            return;
        }

        if (string.IsNullOrWhiteSpace(order.Email))
        {
            _logger.LogWarning("Order {OrderId} has no email; receipt skipped.", orderId);
            return;
        }

        var model = MapToModel(order);
        var subject = OrderReceiptEmailRenderer.RenderSubject(model);
        var html = OrderReceiptEmailRenderer.RenderHtml(model, _receiptOptions);

        var sent = await _sender.SendAsync(order.Email, subject, html, cancellationToken);
        if (!sent)
        {
            return;
        }

        order.ReceiptSentAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order, cancellationToken);
        _logger.LogInformation("Order receipt sent for {OrderId} to {Email}.", orderId, order.Email);
    }

    private static OrderReceiptModel MapToModel(Order order)
    {
        var pickupLocal = order.PickupDateTime.ToLocalTime();
        return new OrderReceiptModel
        {
            OrderId = order.Id,
            ShortOrderId = order.Id.ToString("N")[..8].ToUpperInvariant(),
            CustomerName = order.CustomerName,
            CustomerEmail = order.Email,
            Phone = order.Phone,
            Message = order.Message,
            Location = order.Location,
            PickupDate = pickupLocal.ToString("yyyy-MM-dd"),
            PickupTime = pickupLocal.ToString("HH:mm"),
            Total = order.Total,
            TaxAmount = order.TaxAmount,
            Lines = order.Items.Select(i => new OrderReceiptLineModel
            {
                Name = i.Name,
                OptionLabel = i.OptionLabel,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
