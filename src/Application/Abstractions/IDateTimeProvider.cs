namespace Application.Abstractions
{
    // TODO: Implementera i Infrastructure (UtcNow)
    // TODO: Använd i handlers istället för DateTime.UtcNow för testbarhet
    
    //Vi använder denna interface för att kunna mocka DateTime i våra tester.
    //Det gör det möjligt att kontrollera tidpunkten i våra tester och göra dem mer förutsägbara.
    //Använden av UtcNow säkerställer att vi alltid arbetar med tid i UTC, vilket är viktigt för att undvika problem med tidszoner.
    
    //Syftet med en inteface är att definiera en kontrakt som klasser kan implementera.
    //Det gör det möjligt att skapa olika implementationer av samma funktionalitet, vilket kan
    //vara användbart för testning, beroendehantering och flexibilitet i koden.
    public interface IDateTimeProvider
    {
    }
}