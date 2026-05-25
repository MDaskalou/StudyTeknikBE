namespace MaisonCalliard.Application.Payments.Dtos;

public sealed class ConfirmPaymentIntentRequest
{
    public Guid? OrderId { get; set; }
    public string? PaymentIntentId { get; set; }
}
