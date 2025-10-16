using Domain.Models.Common;

namespace Domain.Entities
{
    public sealed class UserEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        public string FirstName { get; set; } = default!;
        public string LastName  { get; set; } = default!;
        public string SecurityNumber { get; set; } = default!; 
        public string Email { get; set; } = default!;
        public Role Role { get; set; } 
        
        public string? ExternalProvider { get; set; } = default!;
        public string? ExternalSubject  { get; set; } = default!;
        
        public bool ConsentGiven { get; set; }
        public DateTime? ConsentGivenAtUtc { get; set; }
        public string? ConsentSetBy { get; set; }
    }
}