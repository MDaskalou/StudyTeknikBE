namespace MaisonCalliard.Application.Payments.Dtos;

public sealed class CreatePaymentSessionResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
