using Domain.Common;

namespace Domain.Users
{
    //Todo: VO för samtycke
    //Todo: immunt(oföränderligt): skapa nytt om ändring krävs
    //Todo: Spara vem som satte samtycke, när,  för spårbarhet
    //Value Object (om eleven har gett samtycke eller ej).
    
    // sealed class är för att förhindra ärvning
    // Varför vi sätter sealed på studentconsent är för att 
    //vi vill inte att någon annan klass ska ärva från den här klassen.
    //Det är en designbeslut för att säkerställa att klassen förblir
    //oförändrad och inte kan utökas med ytterligare funktionalitet.
    public sealed class StudentConsent : ValueObject
    {
        
    }
}