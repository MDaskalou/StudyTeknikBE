namespace Application.Abstractions.IPersistence.Repositories
{
    //Todo: implementera i Web(httpcontext.User -> ClaimsPrincipal)
    //Claims är en del av säkerhetsmodellen i .Net och används för att representera information om en användare.
    //Claims kan innehålla olika typer av information, såsom användarens namn, roller, e-postadress och andra attribut.
    //Claims används ofta i autentisering och auktorisering för att bestämma vad en användare har tillgång till i en applikation.
    
    // TODO: Säkerställ mapping av roller/Id från IdP (issuer, claim-keys)

    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? ExternalId { get; }
        string? RoleName { get; }
        void SetUserId(Guid id);
        
    }
}