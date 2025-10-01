using Domain.Abstractions.Enum;

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
        public UserRole Role { get; set; } 
        
        public string ExternalProvider { get; set; } = default!;
        public string ExternalSubject  { get; set; } = default!;
        
        public bool ConsentGiven { get; set; }
        public DateTime? ConsentGivenAtUtc { get; set; }
        public string? ConsentSetby { get; set; }
    }
}