using MaisonCalliard.Domain.Enums;

namespace MaisonCalliard.Application.Orders.Dtos;

public sealed class OrderDto
{
    public Guid Id { get; set; }
    public string? StripeSessionId { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
    public decimal Total { get; set; }
    public decimal TaxAmount { get; set; }
    public DateTime PickupDateTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Message { get; set; }
    public OrderStatus Status { get; set; }
    public bool IsPrinted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class CartItemDto
{
    public string CartId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string OptionLabel { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
