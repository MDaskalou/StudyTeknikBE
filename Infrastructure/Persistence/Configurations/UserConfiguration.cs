namespace Infrastructure.Persistence.Configurations
{
    // TODO: Unik e-post. Maxlängder. Indexering.
    // Den här klassen ska konfigurera entiteten User med hjälp av Fluent API.
    // Den ska innehålla all nödvändig konfiguration för att mappa User-entiteten till databastabellen.
    // T.ex. Tabellnamn, primärnyckel, egenskaper, relationer etc.
    
    //Vi använder Configurations för att hålla vår konfigurationslogik separat från vår DbContext-klass.
    // Detta gör koden mer organiserad och lättare att underhålla.
    // Det gör det också enklare att återanvända konfigurationer om vi har flera DbContext-klasser i applikationen.
    
    //Configurations används tillsammans med OnModelCreating-metoden i DbContext-klassen.
    // I OnModelCreating kan vi anropa ApplyConfiguration-metoden för att applicera konfigurationen från en separat klass.
    // Detta gör det möjligt att hålla vår DbContext-klass ren och fokuserad på att hantera databaskontexten.
    
    public class UserConfiguration
    {
        
    }
}