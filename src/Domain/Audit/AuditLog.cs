namespace Domain.Audit
{
    // Revisionslogg: vem gjorde vad, när (UTC).
    public sealed class AuditLog
    {
        // Identitet & metadata
        public Guid Id { get; private set; }
        public DateTime TimestampUtc { get; private set; }

        // Vem (kan vara null för systemhändelser)
        public Guid? UserId { get; private set; }

        // Vad (kort nyckel, t.ex. "Auth.Login", "Diary.Upsert")
        public string EventType { get; private set; } = default!;

        // Extra data som JSON (PII ska maskas)
        public string PayloadJson { get; private set; } = "{}";

        // För att spåra ett helt request/flow
        public string? CorrelationId { get; private set; }

        private AuditLog() { } // För EF

        public AuditLog(string eventType, string? payloadJson, Guid? userId, string? correlationId = null)
        {
            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentException("EventType krävs.", nameof(eventType));

            Id = Guid.NewGuid();
            TimestampUtc = DateTime.UtcNow;

            EventType = eventType.Trim();
            PayloadJson = string.IsNullOrWhiteSpace(payloadJson) ? "{}" : payloadJson.Trim();
            UserId = userId;
            CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? null : correlationId.Trim();
        }
    }
}