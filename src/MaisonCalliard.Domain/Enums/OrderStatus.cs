namespace MaisonCalliard.Domain.Enums;

public enum OrderStatus
{
    Pending,
    Completed,
    Paid,
    /// <summary>Order created at checkout; not visible in kitchen until payment succeeds.</summary>
    AwaitingPayment
}
