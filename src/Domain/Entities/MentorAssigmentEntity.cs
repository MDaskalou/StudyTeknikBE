namespace Domain.Entities
{
    public class MentorAssigmentEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        public Guid MentorId { get; set; }
        public Guid StudentId { get; set; }
    }
}