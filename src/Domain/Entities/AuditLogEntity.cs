namespace Domain.Entities
{
    public class AuditLogEntity()
    {
        public Guid Id { get; set; }
        public DateTime TimestampUtc { get; set; }
        public Guid? UserId { get; set; }
        public string EventType { get; set; } = default!;
        public string PayloadJson { get; set; } = "{}";
        public string? CorrelationId { get; set; }
    }
}