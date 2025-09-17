// Infrastructure/Service/AuditLogger.cs
using Application.Abstractions;                 // IAuditLogger
using Domain.Entities;                          // AuditLogEntity
using Infrastructure.Persistence;               // AppDbContext

namespace Infrastructure.Service
{
    public sealed class AuditLogger : IAuditLogger
    {
        private readonly AppDbContext _db;      // Byt från IAppDbContext till AppDbContext för nu

        public AuditLogger(AppDbContext db) => _db = db;

        public async Task LogAsync(
            string eventType,
            string? payloadJson,
            Guid? userId,
            string? correlationId = null,
            CancellationToken ct = default)
        {
            var payload = string.IsNullOrWhiteSpace(payloadJson) ? "{}" : payloadJson.Trim();

            var log = new AuditLogEntity
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                PayloadJson = payload,
                UserId = userId,
                CorrelationId = correlationId,
                TimestampUtc = DateTime.UtcNow
            };

            await _db.AuditLogs.AddAsync(log, ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}