

namespace Domain.Common
{
    
    //Bas ValueObject som alla ValueObjects i domänen ärver från
    //jämförs på värde exempeel elevens samtycke 
    
    //Todo: Bas för Value Objects
    //Todo: Implementera GetEqualityComponents() i subklasser
    //Todo: Gör dem oföränderliga (immutable)



    public abstract class ValueObject
    {
        
    }
    
    
    public sealed class StudentConsent : ValueObject
    {
        
    }
}