namespace MaisonCalliard.Application.Payments.Dtos;

public sealed class CreatePaymentSessionRequest
{
    public List<PaymentLineItemDto> LineItems { get; set; } = [];
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
}

public sealed class PaymentLineItemDto
{
    public string Name { get; set; } = string.Empty;
    public long UnitAmountOre { get; set; }
    public int Quantity { get; set; }
}
