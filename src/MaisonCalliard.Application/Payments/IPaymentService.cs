using MaisonCalliard.Application.Payments.Dtos;

namespace MaisonCalliard.Application.Payments;

public interface IPaymentService
{
    Task<CreatePaymentSessionResponse> CreateSessionAsync(CreatePaymentSessionRequest request, CancellationToken cancellationToken = default);
    Task<CreatePaymentIntentResponse> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken = default);
    Task HandleWebhookAsync(string payload, string stripeSignature, CancellationToken cancellationToken = default);
    Task<Guid> ConfirmPaymentIntentAsync(ConfirmPaymentIntentRequest request, CancellationToken cancellationToken = default);
}
