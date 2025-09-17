namespace Domain.Entities
{
    public class EnrollmentEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        public Guid StudentId { get; set; } // User.Id
        public Guid ClassId   { get; set; } // Class.Id
        
    }
}