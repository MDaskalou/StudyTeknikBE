namespace MaisonCalliard.Application.Payments.Dtos;

public sealed class CreatePaymentIntentRequest
{
    public Guid OrderId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
}
