namespace Application.Security.RBAC
{
    // TODO: Namnge policies centralt (återanvänd i Web-Auth)
    
    //Syftet med Policies är att gruppera krav (Requirements)
    //som kan återanvändas i olika delar av applikationen.
    //Exempelvis kan en "AdminPolicy" innehålla krav som endast administratörer uppfyller.
    //Dessa policies kan sedan appliceras på controllers eller actions
    //för att säkerställa att endast användare med rätt behörigheter får åtkomst.
    public class Policies
    {
        
    }
}