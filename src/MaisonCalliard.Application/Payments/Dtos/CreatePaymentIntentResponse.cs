namespace MaisonCalliard.Application.Payments.Dtos;

public sealed class CreatePaymentIntentResponse
{
    public string ClientSecret { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
}
