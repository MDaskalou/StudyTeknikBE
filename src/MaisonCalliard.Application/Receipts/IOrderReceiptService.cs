namespace MaisonCalliard.Application.Receipts;

public interface IOrderReceiptService
{
    /// <summary>
    /// Sends order confirmation email once per order (idempotent via ReceiptSentAt).
    /// </summary>
    Task TrySendReceiptAsync(Guid orderId, CancellationToken cancellationToken = default);
}
