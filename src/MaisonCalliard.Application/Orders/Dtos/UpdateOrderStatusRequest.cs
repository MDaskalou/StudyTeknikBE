using MaisonCalliard.Domain.Enums;

namespace MaisonCalliard.Application.Orders.Dtos;

public sealed class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}
