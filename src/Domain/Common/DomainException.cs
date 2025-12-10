using System;

namespace Domain.Common
{
    // Vi ärver från vanliga Exception, men döper den unikt 
    // så vi vet att detta är ett "Business Rule"-fel.
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }
}