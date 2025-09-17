namespace Application.Abstractions
{
    // TODO: Implementeras i Infrastructure (skriv AuditLog + ev. structured logging)
    // TODO: Kallas från kritiska handlers (skapande/uppdatering)
    
    //Syftet med den här interfacen är att definiera en kontrakt för audit-loggning.
    //Audit-loggning syftar på processen att registrera och lagra information om händelser
    //och aktiviteter i ett system för att kunna spåra och granska dem vid behov.
    
    //MVP- en metod som loggar asynkront en händelse med typ, nyttolast (json), användar-id och korrelations-id.

    public interface IAuditLogger
    {
        Task LogAsync(
            string EventType,
            string? payloadJson,
            Guid? userId,
            string? correlationId = null,
            CancellationToken ct = default);
        
    }
}